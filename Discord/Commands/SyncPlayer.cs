using Discord.Providers;
using Discord.Util;

using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

using Shared.Data.Entities;
using Shared.Schemas.Supervive.Private;
using Shared.Services;

namespace Discord.Commands {
	public class SyncPlayer(IDataIntegrationService dataIntegration, ISuperviveService supervive) :ApplicationCommandModule<ApplicationCommandContext> {
		[SlashCommand("sync_player", "Sincroniza os dados de um único usuário (player_id e player_id_encoded)")]
		public async Task RunAsync(
			[SlashCommandParameter(AutocompleteProviderType = typeof(PlayerUniqueDisplayNameAutocompleteProvider))]
			string uniqueDisplayName) {
			await this.RespondAsync(InteractionCallback.DeferredMessage());

			PrivatePlayerData[] foundPlayers = await supervive.SearchPlayers(uniqueDisplayName);
			if (foundPlayers.Length == 0) {
				await this.ModifyResponseAsync(m => m.Embeds = [
					Embeds.WarningEmbed.WithDescription($"Player '{uniqueDisplayName}' não encontrado.")
				]);
				return;
			}

			Player[] players = await dataIntegration.SyncPlayer(uniqueDisplayName);

			string message = $"Player '{uniqueDisplayName}' sincronizado com sucesso com o(s) seguinte(s) id(s) `{string.Join("`, `", players.Select(p => $"{p.Platform}-{p.PlayerId}"))}`";
			await this.ModifyResponseAsync(m => m.Embeds = [Embeds.SuccessEmbed.WithDescription(message)]);
		}
	}
}