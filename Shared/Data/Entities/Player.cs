using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shared.Data.Entities.Supervive;

namespace Shared.Data.Entities {
	
	[Index(nameof(DiscordUserId))]
	[Index(nameof(CreatedAt))]
	[Table("player")]
	public class Player :IEntityTypeConfiguration<Player> {
		
		#region Ids
		
		[Key]
		[Column("player_id")]
		[JsonPropertyName("player_id")]
		public required string PlayerId { get; set; }
		
		[Column("player_id_encoded")]
		[JsonPropertyName("player_id_encoded")]
		public required string PlayerIdEncoded { get; set; }
		
		[Column("discord_user_id")]
		[JsonPropertyName("discord_user_id")]
		public ulong? DiscordUserId { get; set; }
		
		[Column("platform")]
		[JsonPropertyName("platform")]
		public required string Platform { get; set; }
		
		#endregion
		
		[Column("last_synced_match")]
		[JsonPropertyName("last_synced_match")]
		public string? LastSyncedMatch { get; set; }
		
		#region Entity stats
		
		[Column("created_at")]
		[JsonPropertyName("created_at")]
		public DateTime CreatedAt { get; set; }
		
		[Column("deleted_at")]
		[JsonPropertyName("deleted_at")]
		public DateTime? DeletedAt { get; set; }

		#endregion
		
		#region Relationships
		
		[JsonPropertyName("matches_played")]
		[InverseProperty(nameof(MatchPlayer.Player))]
		public ICollection<MatchPlayer> MatchesPlayed { get; set; } = [];
		
		[JsonPropertyName("matches_played_advanced_stats")]
		[InverseProperty(nameof(MatchPlayer.Player))]
		public ICollection<MatchPlayerAdvancedStats> MatchesPlayedAdvancedStats { get; set; } = [];
		
		#endregion
		
		public void Configure(EntityTypeBuilder<Player> builder) {
			builder.Property(p => p.CreatedAt)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP");
			
			builder.HasAlternateKey(p => p.PlayerIdEncoded);
			
			builder.HasMany(p => p.MatchesPlayed)
				   .WithOne(mp => mp.Player)
				   .HasForeignKey(mp => mp.PlayerIdEncoded)
				   .HasPrincipalKey(sp => sp.PlayerIdEncoded)
				   .OnDelete(DeleteBehavior.NoAction);
			
			builder.HasMany(p => p.MatchesPlayedAdvancedStats)
				   .WithOne(mp => mp.Player)
				   .HasForeignKey(mp => mp.PlayerId)
				   .OnDelete(DeleteBehavior.NoAction);
		}
	}
}
