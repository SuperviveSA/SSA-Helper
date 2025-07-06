using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace Discord.Commands {
	public class PingCommand(GatewayClient gateway) :ApplicationCommandModule<ApplicationCommandContext> {
		[SlashCommand("ping", "Shows latency")]
		public InteractionCallback<InteractionMessageProperties> Run() => InteractionCallback.Message(new InteractionMessageProperties {
			Content = $"Pong! Latency is {gateway.Latency.TotalMilliseconds}ms",
			Components = [
				new ActionRowProperties {
					new ButtonProperties("ping_refresh", new EmojiProperties(1388078336051253469), ButtonStyle.Secondary)
				}
			]
		});
	}
}