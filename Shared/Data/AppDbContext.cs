using Microsoft.EntityFrameworkCore;

using Shared.Data.Entities;
using Shared.Data.Entities.Supervive;

namespace Shared.Data {
	public class AppDbContext(DbContextOptions<AppDbContext> options) :DbContext(options) {
		public DbSet<Player>                   Players                   { get; init; }
		public DbSet<Match>                    Matches                   { get; init; }
		public DbSet<MatchPlayer>              MatchPlayers              { get; init; }
		public DbSet<MatchPlayerAdvancedStats> MatchPlayersAdvancedStats { get; init; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
		}
	}
}