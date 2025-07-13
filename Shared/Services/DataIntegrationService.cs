using System.Security.Cryptography;
using System.Text;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Shared.Data;
using Shared.Data.Entities;
using Shared.Data.Entities.Supervive;
using Shared.Schemas.Supervive;
using Shared.Schemas.Supervive.Private;
using Shared.Schemas.Supervive.Public;

namespace Shared.Services {
	public interface IDataIntegrationService {
		Task<int>      SyncPlayerMatches(string platform,          string playerId, int syncDelayMillis = 2000);
		Task<Player[]> SyncPlayer(string        uniqueDisplayName, bool   saveChanges = true);
	}

	public class DataIntegrationService(ISuperviveService supervive, AppDbContext ctx) :IDataIntegrationService {
		public static void ConfigureService(IServiceCollection services) {
			services.AddScoped<IDataIntegrationService, DataIntegrationService>();
		}

		public async Task<int> SyncPlayerMatches(string platform, string playerId, int syncDelayMillis = 2000) {
			await supervive.FetchNewPlayerMatches(platform, playerId);

			await Task.Delay(syncDelayMillis);

			DataResponse<PrivateMatchData> matches = await supervive.GetPlayerMatches(platform, playerId);
			if (matches.Data.Length == 0) throw new Exception($"No Matches found for player '{playerId}' on '{platform}'");

			Player             player         = await this.UpsertPlayer(SuperviveDataAdapter.PlayerMatchDataToPlayerDb(matches.Data.First()));
			PrivateMatchData[] pendingMatches = await this.GetPendingSyncMatches(player, matches);

			// Sync in reverse to be able to gracefully retry in case of error
			pendingMatches = pendingMatches.Reverse().ToArray();
			foreach (PrivateMatchData privateData in pendingMatches) {
				if (await ctx.Matches.AnyAsync(m => m.MatchId == privateData.MatchId)) continue;

				PublicMatchData[] publicData = await supervive.GetMatch(platform, privateData.MatchId);

				ctx.Matches.Add(SuperviveDataAdapter.MatchDataToDb(privateData, publicData));
				ctx.MatchPlayersAdvancedStats.Add(SuperviveDataAdapter.PrivateMatchDataAdvancedStatsToDb(privateData.MatchId, privateData));

				foreach (PublicMatchData individualPublicData in publicData) {
					if (individualPublicData.Player.UniqueDisplayName is null) continue;
					await this.SyncPlayer(individualPublicData.Player.UniqueDisplayName, false);

					await this.UpsertMatchPlayer(SuperviveDataAdapter.IndividualMatchDataToDb(privateData.MatchId, individualPublicData));
				}

				player.LastSyncedMatch = privateData.MatchId;
				await ctx.SaveChangesAsync();
			}

			return pendingMatches.Length;
		}

		private async Task<PrivateMatchData[]> GetPendingSyncMatches(Player player, DataResponse<PrivateMatchData>? currentReq = null) {
			string?                lastSyncedMatch = player.LastSyncedMatch;
			List<PrivateMatchData> matches         = [];

			DataResponse<PrivateMatchData> current     = currentReq ?? await supervive.GetPlayerMatches(player.Platform, player.PlayerId);
			int                            currentPage = current.Meta.CurrentPage;

			// While no current page does not contain last synced && there are pages left
			while (current.Data.All(m => m.MatchId != lastSyncedMatch) && currentPage <= current.Meta.LastPage) {
				matches.AddRange(current.Data.TakeWhile(m => m.MatchId != lastSyncedMatch));

				current = await supervive.GetPlayerMatches(player.Platform, player.PlayerId, ++currentPage);
			}

			return matches.ToArray();
		}

		public async Task<Player[]> SyncPlayer(string uniqueDisplayName, bool saveChanges = true) {
			PrivatePlayerData[] searchResult = await supervive.SearchPlayers(uniqueDisplayName);

			if (searchResult.Length == 0)
				throw new Exception($"Player not found for query '{uniqueDisplayName}'");

			List<Player> syncedPlayers = [];
			foreach (PrivatePlayerData player in searchResult)
				syncedPlayers.Add(await this.UpsertPlayer(new Player {
					PlayerId        = player.UserId,
					PlayerIdEncoded = EncodePlayerId(player.UserId),
					Platform        = player.Platform
				}));

			if (saveChanges) await ctx.SaveChangesAsync();

			return syncedPlayers.ToArray();
		}

		private async Task UpsertMatchPlayer(MatchPlayer matchPlayerData) {
			MatchPlayer? existingMatchPlayer = await ctx.MatchPlayers.FindAsync(matchPlayerData.MatchId, matchPlayerData.PlayerIdEncoded);

			if (existingMatchPlayer is null) ctx.MatchPlayers.Add(matchPlayerData);
			else {
				ctx.Entry(existingMatchPlayer).CurrentValues.SetValues(matchPlayerData);
				ctx.Entry(existingMatchPlayer).Property(p => p.CreatedAt).IsModified = false;
			}
		}

		private async Task<Player> UpsertPlayer(Player playerData) {
			Player? existingPlayer = await ctx.Players.FindAsync(playerData.PlayerId);

			if (existingPlayer is null) ctx.Players.Add(playerData);
			else {
				ctx.Entry(existingPlayer).CurrentValues.SetValues(playerData);
				ctx.Entry(existingPlayer).Property(p => p.CreatedAt).IsModified = false;
			}

			return existingPlayer ?? playerData;
		}

		private static string EncodePlayerId(string playerId) {
			byte[] inputBytes = Encoding.UTF8.GetBytes(playerId);
			byte[] hashBytes  = MD5.HashData(inputBytes);

			StringBuilder sb = new(hashBytes.Length * 2);
			foreach (byte b in hashBytes)
				sb.Append(b.ToString("x2")); // "x2" for lowercase hex, "X2" for uppercase

			return sb.ToString();
		}
	}
}