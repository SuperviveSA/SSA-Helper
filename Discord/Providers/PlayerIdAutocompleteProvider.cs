using NetCord;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

using Shared.Schemas.Supervive.Private;
using Shared.Services;

namespace Discord.Providers {
	public class PlayerIdAutocompleteProvider(ISuperviveService supervive) :IAutocompleteProvider<AutocompleteInteractionContext> {
		public async ValueTask<IEnumerable<ApplicationCommandOptionChoiceProperties>?> GetChoicesAsync(ApplicationCommandInteractionDataOption option, AutocompleteInteractionContext context) {
			if (option.Value is null) return null;

			PrivatePlayerData[] players = await supervive.SearchPlayers(option.Value);
			
			return players.Select(p => new ApplicationCommandOptionChoiceProperties($"{p.Platform} - {p.UniqueDisplayName}", $"{p.Platform}-{p.UserId}"));
		}
	}
}
