using Discord.Providers;
using Discord.Services;
using Discord.Util;

using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

using Shared.Schemas.Supervive;
using Shared.Schemas.Supervive.Private;
using Shared.Schemas.Supervive.Public;
using Shared.Services;

namespace Discord.Commands {
	public class CalculateTournamentPoints(ISuperviveService supervive, ITournamentService tournament, ITournamentHelperService tournamentHelper) :ApplicationCommandModule<ApplicationCommandContext> {
		[SlashCommand("calc_tournament_points", "Calcula os pontos por partida de cada time")]
		public async Task RunAsync(
			[SlashCommandParameter(Description = "Player to get matches from", AutocompleteProviderType = typeof(PlayerNameProvider))]
			string? playerName = null,
			[SlashCommandParameter(Description = "Number of matches to use when calculating points", MinValue = 1, MaxValue = 15)]
			int matchCount = 1,
			[SlashCommandParameter(Description = "Attach csv data")]
			bool attachData = false,
			[SlashCommandParameter(Description = "Match id list sepparated by ,")]
			string? matchIds = null,
			[SlashCommandParameter]
			string platform = "steam",
			[SlashCommandParameter(Description = "Number of teams to display in the result embed")]
			int topTeams = 3
		) {
			await this.RespondAsync(InteractionCallback.DeferredMessage());

			PublicMatchData[][] matches;
			if (matchIds is null) {
				if (playerName is null) {
					await this.ModifyResponseAsync(m => {
						m.Embeds = [Embeds.ErrorEmbed.WithDescription("PlayerName cannot be null when not passing matchIds")];
					});

					return;
				}

				PrivatePlayerData              playerData = (await supervive.SearchPlayers(playerName)).First(p => p.Platform == platform);
				DataResponse<PrivateMatchData> allMatches = await supervive.GetPlayerMatches(platform, playerData.UserId);

				matches = await Task.WhenAll(allMatches
											.Data
											.Take(matchCount)
											.Select(async m =>
														await supervive.GetMatch(platform, m.MatchId))
											.ToArray());
			} else
				matches = await Task.WhenAll(matchIds
											.Split(',')
											.Select(async m =>
														await supervive.GetMatch(m.Split('-')[0], string.Join('-', m.Split('-').TakeLast(5))))
											.ToArray());

			Dictionary<string, int> placement         = tournament.CalculateTeamPoints(matches);
			PublicMatchData[]       summedPlayerStats = tournament.SumPlayerStats(matches);

			await this.ModifyResponseAsync(m => {
				m.Embeds = [tournamentHelper.BuildTournamentResultEmbed(placement, summedPlayerStats, topTeams)];
				if (attachData) m.Attachments = [tournamentHelper.BuildTournamentResultCsv(summedPlayerStats)];
			});
		}
	}
}