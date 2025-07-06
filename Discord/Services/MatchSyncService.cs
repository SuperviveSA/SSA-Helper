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
		Task<EmbedProperties> GetUpdatedSyncEmbed(string[] playerIds);
	}

	public class MatchSyncService(ISuperviveService supervive, AppDbContext ctx) :IMatchSyncService {
		public static void ConfigureService(IServiceCollection services) {
			services.AddScoped<IMatchSyncService, MatchSyncService>();
		}

		public async Task<EmbedProperties> GetUpdatedSyncEmbed(string[] playerIds) {
			List<EmbedFieldProperties> fields = [];
			foreach (string fullPlayerId in playerIds) {
				string platform = fullPlayerId.Split('-')[0];
				string playerId = fullPlayerId.Split('-')[1];

				Task<int> totalMatches = ctx.Matches
											.Where(m => m.Players.Any(mp => mp.Player!.PlayerId == playerId))
											.CountAsync();

				Task<DataResponse<PrivateMatchData>> data = supervive.GetPlayerMatches(platform, playerId);

				await Task.WhenAll(totalMatches, data);
				int   total   = data.Result.Meta.Total;
				int   done    = totalMatches.Result;
				float percent = (float)done / total * 100;

				fields.Add(new EmbedFieldProperties {
					Name   = $"Progresso: *{done}/{total}* {percent:f2}%",
					Value  = fullPlayerId,
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