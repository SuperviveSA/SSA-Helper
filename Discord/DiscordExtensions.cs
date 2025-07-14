using Discord.Commands;
using Discord.Commands.Interactions;
using Discord.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using NetCord;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services.ApplicationCommands;
using NetCord.Hosting.Services.ComponentInteractions;
using NetCord.Services.ComponentInteractions;

namespace Discord {
	public class DiscordExtensions {
		public static void ConfigureService(WebApplicationBuilder builder) {
			builder.Services.AddDiscordGateway(options => {
				options.Intents = GatewayIntents.GuildMessages | GatewayIntents.MessageContent;
				options.Token   = builder.Configuration.GetValue<string>("discord-token");
			});

			builder.Services.AddApplicationCommands(options => {
				options.ResultHandler = new DefaultResultHandler();
			});
			builder.Services.AddComponentInteractions<ButtonInteraction, ButtonInteractionContext>();

			builder.Services.AddSingleton<PingCommand>();

			MatchSyncService.ConfigureService(builder.Services);
			TournamentHelperService.ConfigureService(builder.Services);
		}

		public static void ConfigureHost(WebApplication host) {
			host.UseGatewayHandlers();

			host.AddApplicationCommandModule<PingCommand>();
			host.AddComponentInteractionModule<PingCommandInteractions>();
			host.AddApplicationCommandModule<SyncPlayer>();

			host.AddApplicationCommandModule<SyncMatches>();
			host.AddComponentInteractionModule<SyncMatchesCommandInteractions>();

			host.AddApplicationCommandModule<RunMigrations>();
			
			host.AddApplicationCommandModule<CalculateTournamentPoints>();
			host.AddApplicationCommandModule<Compare>();
		}
	}
}