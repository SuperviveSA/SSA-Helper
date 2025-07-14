using Discord.Providers;
using Discord.Services;
using Discord.Util;

using Microsoft.EntityFrameworkCore;

using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

using Shared.Data;
using Shared.Data.Entities;
using Shared.Schemas.Supervive;
using Shared.Schemas.Supervive.Private;
using Shared.Services;

using MatchType = Shared.Data.Entities.Supervive.MatchType;

namespace Discord.Commands {
	public class Compare(AppDbContext ctx, ISuperviveService supervive, IDataIntegrationService integration, IMatchSyncService sync) :ApplicationCommandModule<ApplicationCommandContext> {
		[SlashCommand("compare", "Compara o winrate de dois players")]
		public async Task RunAsync(
			[SlashCommandParameter(Description = "First player to compare", AutocompleteProviderType = typeof(PlayerNameProvider))]
			string player1,
			[SlashCommandParameter(Description = "Second player to compare", AutocompleteProviderType = typeof(PlayerNameProvider))]
			string player2,
			[SlashCommandParameter(Description = "Hero from first player to compare")]
			SuperviveHero? hero1 = null,
			[SlashCommandParameter(Description = "Hero from second player to compare")]
			SuperviveHero? hero2 = null,
			[SlashCommandParameter(Description = "Platform to get players from")]
			string platform = "steam"
		) {
			await this.RespondAsync(InteractionCallback.DeferredMessage());

			if (player1.Equals(player2, StringComparison.OrdinalIgnoreCase)) {
				await this.ModifyResponseAsync(m => {
					m.Embeds = [Embeds.ErrorEmbed.WithDescription("❌ Os jogadores precisam ser diferentes.")];
				});
				return;
			}

			EmbedProperties syncEmbed = await sync.GetUpdatedSyncEmbed([player1, player2], platform);
			await this.ModifyResponseAsync(m => {
				m.Embeds = [syncEmbed];
				m.Components = [
					new ActionRowProperties {
						new ButtonProperties($"sync_matches_refresh:{platform}:{player1}:{player2}",
											 new EmojiProperties(1388078336051253469), ButtonStyle.Secondary)
					}
				];
			});

			PrivatePlayerData player1Data = (await supervive.SearchPlayers(player1)).First(p => p.Platform == platform);
			PrivatePlayerData player2Data = (await supervive.SearchPlayers(player2)).First(p => p.Platform == platform);
			string            id1         = player1Data.UserId;
			string            id2         = player2Data.UserId;

			await integration.SyncPlayerMatches(platform, id1);
			await integration.SyncPlayerMatches(platform, id2);

			Dictionary<string, Player> players = await ctx.Players.AsNoTracking()
														  .Where(p => p.PlayerId == id1 || p.PlayerId == id2)
														  .ToDictionaryAsync(p => p.PlayerId);

			if (!players.TryGetValue(id1, out Player? dbPlayer1)) {
				await this.ModifyResponseAsync(m => {
					m.Embeds = [Embeds.ErrorEmbed.WithDescription("❌ Player 2 não encontrado.")];
				});
				return;
			}

			if (!players.TryGetValue(id2, out Player? dbPlayer2)) {
				await this.ModifyResponseAsync(m => {
					m.Embeds = [Embeds.ErrorEmbed.WithDescription("❌ Player 2 não encontrado.")];
				});
				return;
			}

			string? heroInternal1 = hero1 is null ? null : SuperviveDataAdapter.GetInternalHeroName(hero1.Value);
			string? heroInternal2 = hero2 is null ? null : SuperviveDataAdapter.GetInternalHeroName(hero2.Value);

			var stats = await (
					from mp1 in ctx.MatchPlayers.AsNoTracking()
					join mp2 in ctx.MatchPlayers.AsNoTracking() on mp1.MatchId equals mp2.MatchId
					join m in ctx.Matches.AsNoTracking() on mp1.MatchId equals m.MatchId
					where
						mp1.PlayerIdEncoded == dbPlayer1.PlayerIdEncoded &&
						mp2.PlayerIdEncoded == dbPlayer2.PlayerIdEncoded &&
						mp1.TeamId          != mp2.TeamId                &&
						(hero1 == null || mp1.Hero == heroInternal1)     &&
						(hero2 == null || mp2.Hero == heroInternal2)
					select new {
						m.Type,
						m.WinnerTeam,
						P1Team      = mp1.TeamId,
						P2Team      = mp2.TeamId,
						P1Placement = mp1.Placement,
						P2Placement = mp2.Placement
					}
				).GroupBy(_ => 1)
				 .Select(g => new {
					  TotalTrios  = g.Count(x => x.Type == MatchType.Trios),
					  TotalArena  = g.Count(x => x.Type == MatchType.Arena),
					  P1WinsTrios = g.Count(x => x.P1Placement > x.P2Placement && x.Type == MatchType.Trios),
					  P1WinsArena = g.Count(x => x.WinnerTeam  == x.P1Team     && x.Type == MatchType.Arena),
					  P2WinsTrios = g.Count(x => x.P2Placement > x.P1Placement && x.Type == MatchType.Trios),
					  P2WinsArena = g.Count(x => x.WinnerTeam  == x.P2Team     && x.Type == MatchType.Arena)
				  }).SingleAsync();

			EmbedProperties embed = new() {
				Title       = $"{player1} vs {player2}",
				Description = BuildDescription(hero1, hero2),
				Color       = Colors.DefaultGreen,
				Fields = [
					new EmbedFieldProperties {
						Name = "Partidas",
						Value = $"Arena: {stats.TotalArena}\n" +
								$"Trios: {stats.TotalTrios}",
						Inline = true
					},
					new EmbedFieldProperties {
						Name = "Vitórias P1",
						Value = $"{stats.P1WinsArena} ({Percent(stats.P1WinsArena, stats.TotalArena)})\n" +
								$"{stats.P1WinsTrios} ({Percent(stats.P1WinsTrios, stats.TotalTrios)})",
						Inline = true
					},
					new EmbedFieldProperties {
						Name = "Vitórias P2",
						Value = $"{stats.P2WinsArena} ({Percent(stats.P2WinsArena, stats.TotalArena)})\n" +
								$"{stats.P2WinsTrios} ({Percent(stats.P2WinsTrios, stats.TotalTrios)})",
						Inline = true
					}
				],
				Timestamp = DateTimeOffset.UtcNow
			};

			await this.ModifyResponseAsync(m => {
				m.Embeds     = [embed];
				m.Components = [];
			});
		}

		private static string Percent(int part, int total)
			=> total == 0 ? "0 %" : $"{Math.Round(part * 100d / total, 2):0.##} %";

		private static string? BuildDescription(SuperviveHero? h1, SuperviveHero? h2)
			=> h1 is null && h2 is null ? null : $"Herói(s) filtrados(P1 vs P2) » {h1?.ToString() ?? "todos"} vs {h2?.ToString() ?? "todos"}";
	}
}