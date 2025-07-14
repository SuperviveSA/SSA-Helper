using Discord.Util;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using NetCord.Rest;

using Shared.Data;
using Shared.Schemas.Supervive;
using Shared.Schemas.Supervive.Private;
using Shared.Services;

namespace Discord.Services {
	public interface IMatchSyncService {
		Task<EmbedProperties> GetUpdatedSyncEmbed(string[] playerNames, string platform = "steam");
	}

	public class MatchSyncService(ISuperviveService supervive, AppDbContext ctx) :IMatchSyncService {
		public static void ConfigureService(IServiceCollection services) {
			services.AddScoped<IMatchSyncService, MatchSyncService>();
		}

		public async Task<EmbedProperties> GetUpdatedSyncEmbed(string[] playerNames, string platform = "steam") {
			List<EmbedFieldProperties> fields = [];
			foreach (string playerName in playerNames) {
				PrivatePlayerData playerData = (await supervive.SearchPlayers(playerName))
				   .First(p => p.Platform == platform);

				Task<int> totalMatches = ctx.MatchPlayersAdvancedStats
											.Where(m => m.PlayerId == playerData.UserId)
											.CountAsync();

				Task<DataResponse<PrivateMatchData>> data = supervive.GetPlayerMatches(platform, playerData.UserId);

				await Task.WhenAll(totalMatches, data);
				int   total   = data.Result.Meta.Total;
				int   done    = totalMatches.Result;
				float percent = (float)done / total * 100;

				fields.Add(new EmbedFieldProperties {
					Name   = playerData.UniqueDisplayName,
					Value  = $"Progresso: **{done}/{total}** ({percent:f2})%\n{platform}-{playerData.UserId}",
					Inline = false
				});
			}

			return new EmbedProperties {
				Color = Colors.DefaultOrange,
				Author = new EmbedAuthorProperties {
					Name    = "Sincronizando Player(s)...",
					IconUrl = Images.OrangeExclamation
				},
				Fields = fields
			};
		}
	}
}