using NetCord.Rest;
using NetCord.Services.ComponentInteractions;

namespace Discord.Commands.Interactions {
	public class PingCommandInteractions(PingCommand command) : ComponentInteractionModule<ButtonInteractionContext> {
		[ComponentInteraction("ping_refresh")]
		public InteractionCallback<MessageOptions> Run() => InteractionCallback.ModifyMessage(message => {
			message.Content = command.Run().Data.Content;
		});
	}
}
