using Discord.Providers;
using Discord.Services;
using Discord.Util;

using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

using Shared.Services;

namespace Discord.Commands {
	public class SyncMatches(IDataIntegrationService dataIntegration, IMatchSyncService sync) :ApplicationCommandModule<ApplicationCommandContext> {
		[SlashCommand("sync_matches", "Sincroniza todas as partidas de um usuário")]
		public async Task RunAsync(
			[SlashCommandParameter(AutocompleteProviderType = typeof(PlayerIdAutocompleteProvider))]
			string playerId) {

			await this.RespondAsync(InteractionCallback.Message(new InteractionMessageProperties {
				Embeds = [await sync.GetUpdatedSyncEmbed([playerId])],
				Components = [
					new ActionRowProperties {
						new ButtonProperties($"sync_matches_refresh:{playerId}", new EmojiProperties(1388078336051253469), ButtonStyle.Secondary)
					}
				]
			}));

			int count = await dataIntegration.SyncPlayerMatches(playerId.Split('-')[0], playerId.Split('-')[1]);

			await this.ModifyResponseAsync(m => {
				m.Content = null;
				m.Embeds  = [Embeds.SuccessEmbed.WithDescription($"Synced user {playerId} successfully with {count} new matches")];
			});
		}
	}
}