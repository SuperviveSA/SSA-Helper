using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

using Shared.Data;

namespace Discord.Commands {
	public class RunMigrations(AppDbContext ctx, Logger<RunMigrations> logger) :ApplicationCommandModule<ApplicationCommandContext> {
		[SlashCommand("run_migrations", "Runs pending migrations in the database")]
		public async Task<InteractionCallback<InteractionMessageProperties>> Run() {
			IExecutionStrategy strategy = ctx.Database.CreateExecutionStrategy();
			try {
				await strategy.ExecuteAsync(async () => {
					await ctx.Database.MigrateAsync();
				});
			} catch (Exception e) {
				logger.LogError(e, "An error occurred while syncing user matches");

				await this.RespondAsync(InteractionCallback.Message($"Could not run migrations:\n{e.Message}"));
			}
			
			return InteractionCallback.Message("Migrations run successfully");
		}
	}
}