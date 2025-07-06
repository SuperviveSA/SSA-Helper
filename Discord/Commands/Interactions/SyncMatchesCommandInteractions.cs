using Discord.Services;

using NetCord;
using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace Discord.Commands.Interactions {
	public class SyncMatchesCommandInteractions(IMatchSyncService sync) :ComponentInteractionModule<ButtonInteractionContext> {
		[ComponentInteraction("sync_matches_refresh")]
		public async Task RunAsync(params string[] playerIds) {
			await this.RespondAsync(InteractionCallback.DeferredModifyMessage);

			EmbedProperties newEmbed = await sync.GetUpdatedSyncEmbed(playerIds);

			await this.ModifyResponseAsync(m => m.Embeds = [newEmbed]);
		}
	}
}