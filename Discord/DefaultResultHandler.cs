using Discord.Util;

using Microsoft.Extensions.Logging;

using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;

namespace Discord {
	public class DefaultResultHandler(MessageFlags? messageFlags = null) :IApplicationCommandResultHandler<ApplicationCommandContext> {
		public async ValueTask HandleResultAsync(IExecutionResult result, ApplicationCommandContext context, GatewayClient? client, ILogger logger, IServiceProvider services) {
			if (result is not IFailResult failResult) return;

			string                        resultMessage = failResult.Message;
			ApplicationCommandInteraction interaction   = context.Interaction;

			if (failResult is not IExceptionResult exceptionResult) {
				logger.LogDebug("Execution of an application command of name '{Name}' failed with '{Message}'", interaction.Data.Name, resultMessage);

				return;
			}

			logger.LogError(exceptionResult.Exception, "Execution of an application command of name '{Name}' failed with an exception", interaction.Data.Name);

			try {
				await interaction.ModifyResponseAsync(m => {
					m.Content    = null;
					m.Embeds     = [Embeds.CodeError(exceptionResult.Exception)];
					m.Flags      = messageFlags;
					m.Components = [];
				});
			} catch (Exception) {
				await interaction.SendResponseAsync(InteractionCallback.Message(new InteractionMessageProperties {
					Content    = null,
					Embeds = [Embeds.CodeError(exceptionResult.Exception)],
					Flags      = messageFlags,
					Components = []
				}));
			}
		}
	}
}