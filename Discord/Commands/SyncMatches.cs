using Discord.Providers;
using Discord.Services;
using Discord.Util;

using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

using Shared.Schemas.Supervive.Private;
using Shared.Services;

namespace Discord.Commands {
	public class SyncMatches(
		ISuperviveService       supervive,
		IDataIntegrationService dataIntegration,
		IMatchSyncService       sync) :ApplicationCommandModule<ApplicationCommandContext> {
		[SlashCommand("sync_matches", "Sincroniza todas as partidas de um usuário")]
		public async Task RunAsync(
			[SlashCommandParameter(AutocompleteProviderType = typeof(PlayerNameProvider))]
			string playerName,
			[SlashCommandParameter]
			string platform = "steam") {
			await this.RespondAsync(InteractionCallback.Message(new InteractionMessageProperties {
				Embeds = [await sync.GetUpdatedSyncEmbed([playerName], platform)],
				Components = [
					new ActionRowProperties {
						new ButtonProperties($"sync_matches_refresh:{platform}:{playerName}",
											 new EmojiProperties(1388078336051253469), ButtonStyle.Secondary)
					}
				]
			}));

			PrivatePlayerData playerData = (await supervive.SearchPlayers(playerName)).First(p => p.Platform == platform);
			int               count      = await dataIntegration.SyncPlayerMatches(platform, playerData.UserId);

			await this.ModifyResponseAsync(m => {
				m.Content    = null;
				m.Embeds     = [Embeds.SuccessEmbed.WithDescription($"Synced user {playerName} successfully with {count} new matches")];
				m.Components = [];
			});
		}
	}
}