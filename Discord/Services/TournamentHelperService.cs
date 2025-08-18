using System.Text;

using Discord.Util;

using Microsoft.Extensions.DependencyInjection;

using NetCord.Rest;

using Shared.Schemas;
using Shared.Schemas.Supervive.Public;
using Shared.Services;

namespace Discord.Services {
	public interface ITournamentHelperService {
		public EmbedProperties      BuildTournamentResultEmbed(IEnumerable<RosterResult> rosters, int topTeams = 3);
		public AttachmentProperties BuildTournamentResultCsv(IEnumerable<RosterResult>   rosters);
	}

	public class TournamentHelperService(ITournamentService tournamentService) :ITournamentHelperService {
		public static void ConfigureService(IServiceCollection services) {
			services.AddScoped<ITournamentHelperService, TournamentHelperService>();
		}

		public EmbedProperties BuildTournamentResultEmbed(IEnumerable<RosterResult> rosters, int topTeams = 3) {
			List<EmbedFieldProperties> fields = [];
			int                        rank   = 1;

			foreach (RosterResult roster in rosters.OrderByDescending(r => r.Points)) {
				string placementsStr = roster.Placements.Count == 0
					? "-"
					: string.Join(", ", roster.Placements.Select(p => $"{p}º"));

				fields.Add(new EmbedFieldProperties {
					Name   = $"#{rank}: {placementsStr} - {roster.Points}pts",
					Value  = this.BuildPlayerStats(roster.Players.Values),
					Inline = false
				});

				if (rank++ >= topTeams) break;
			}

			return new EmbedProperties {
				Color = Colors.DefaultGreen,
				Author = new EmbedAuthorProperties {
					Name    = "Resultado calculado",
					IconUrl = Images.GreenCheck
				},
				Fields = fields
			};
		}

		public AttachmentProperties BuildTournamentResultCsv(IEnumerable<RosterResult> rosters) {
			const string fileName = "resultado.csv";
			const string header   = "Team\tPlayer\tScore\tKills\tDeaths\tAssists\tHealingGiven\tHealingSelf\tDamageDone\tDamageTaken";

			// MemoryStream is left open because we hand it to AttachmentProperties.
			MemoryStream       stream = new();
			using StreamWriter writer = new(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), leaveOpen: true);

			writer.WriteLine(header);

			foreach (RosterResult roster in rosters) {
				string teamKey = string.Join('|', roster.CanonicalMemberIds.OrderBy(id => id));

				foreach (PublicMatchData p in roster.Players.Values) {
					writer.WriteLine(string.Join('\t',
												 teamKey,
												 p.Player.DisplayName,
												 p.Stats.Kills,
												 p.Stats.Kills,
												 p.Stats.Deaths,
												 p.Stats.Assists,
												 p.Stats.HealingGiven,
												 p.Stats.HealingGivenSelf,
												 p.Stats.HeroEffectiveDamageDone,
												 p.Stats.HeroEffectiveDamageTaken));
				}
			}

			writer.Flush();
			stream.Position = 0; // rewind for the consumer

			return new AttachmentProperties(fileName, stream);
		}

		public string BuildPlayerStats(IEnumerable<PublicMatchData> players) {
			PublicMatchData[] team = players.OrderByDescending(p => p.Stats.Kills).ToArray();

			int colPlayer = Math.Max("player#0000".Length,
									 team.Max(p => LimitPlayerName(p.Player.UniqueDisplayName ?? "").Length)
			) + 1;

			const int colKda    = 8 + 1;
			const int colDamage = 7 + 1;
			const int colTanked = 7 + 1;

			StringBuilder sb = new();

			sb.AppendLine("```");

			sb.Append("Player".PadRight(colPlayer));
			sb.Append("K/D/A".PadRight(colKda));
			sb.Append("Damage".PadRight(colDamage));
			sb.Append("Tanked".PadRight(colTanked));
			sb.Append("Healing");
			sb.AppendLine();

			foreach (PublicMatchData p in team) {
				string kda     = $"{p.Stats.Kills}/{p.Stats.Deaths}/{p.Stats.Assists}";
				double healing = p.Stats.HealingGiven + p.Stats.HealingGivenSelf;

				sb.Append(LimitPlayerName(p.Player.UniqueDisplayName ?? "").PadRight(colPlayer));
				sb.Append(kda.PadRight(colKda));
				sb.Append($"{p.Stats.HeroEffectiveDamageDone:N0}".PadRight(colDamage));
				sb.Append($"{p.Stats.HeroEffectiveDamageTaken:N0}".PadRight(colTanked));
				sb.Append($"{healing:N0}");
				sb.AppendLine();
			}

			sb.Append("```");
			return sb.ToString();
		}

		private static string LimitPlayerName(string name) => name.Length > 16 ? $"{name[..13]}..." : name;
	}
}