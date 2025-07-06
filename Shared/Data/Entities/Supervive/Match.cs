using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Data.Entities.Supervive {
	[Index(nameof(IsRanked))]
	[Index(nameof(IsInhouse))]
	[Index(nameof(InhouseServerId))]
	[Index(nameof(CreatedAt))]
	[Table("match")]
	public class Match :IEntityTypeConfiguration<Match> {
		
		#region Ids
		
		[Key]
		[Column("match_id")]
		[JsonPropertyName("match_id")]
		public required string MatchId { get; set; }
		
		[Column("platform")]
		[JsonPropertyName("platform")]
		public required string Platform { get; set; }
		
		#endregion

		[Column("winner_team")]
		[JsonPropertyName("winner_team")]
		public int? WinnerTeam { get; set; }
		
		[Column("type")]
		[JsonPropertyName("type")]
		public MatchType Type { get; set; }
		
		[Column("is_ranked")]
		[JsonPropertyName("is_ranked")]
		public bool IsRanked { get; set; }
		
		[Column("is_custom_game")]
		[JsonPropertyName("is_custom_game")]
		public bool IsCustomGame { get; set; }
		
		[Column("is_inhouse")]
		[JsonPropertyName("is_inhouse")]
		public bool IsInhouse { get; set; }
		
		[Column("inhouse_server_id")]
		[JsonPropertyName("inhouse_server_id")]
		public ulong? InhouseServerId { get; set; }
		
		[Column("match_start")]
		[JsonPropertyName("match_start")]
		public DateTime MatchStart { get; init; }
		
		[Column("match_end")]
		[JsonPropertyName("match_end")]
		public DateTime MatchEnd { get; init; }

		#region Entity stats
		
		[Column("created_at")]
		[JsonPropertyName("created_at")]
		public DateTime CreatedAt { get; set; }
		
		[Column("deleted_at")]
		[JsonPropertyName("deleted_at")]
		public DateTime? DeletedAt { get; set; }

		#endregion
		
		#region Relationships
		
		[JsonPropertyName("players")]
		[InverseProperty(nameof(MatchPlayer.Match))]
		public ICollection<MatchPlayer> Players { get; set; } = [];
		
		[JsonPropertyName("player_advanced_stats")]
		[InverseProperty(nameof(MatchPlayerAdvancedStats.Match))]
		public ICollection<MatchPlayerAdvancedStats> PlayersAdvancedStats { get; set; } = [];
		
		#endregion

		void IEntityTypeConfiguration<Match>.Configure(EntityTypeBuilder<Match> builder) {
			builder.Property(m => m.Type).HasConversion<string>();
			
			builder.Property(m => m.CreatedAt)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP");

			builder.HasMany(m => m.Players)
				   .WithOne(mp => mp.Match)
				   .HasForeignKey(mp => mp.MatchId)
				   .OnDelete(DeleteBehavior.Cascade);
			
			builder.HasMany(m => m.PlayersAdvancedStats)
				   .WithOne(mp => mp.Match)
				   .HasForeignKey(mp => mp.MatchId)
				   .OnDelete(DeleteBehavior.Cascade);
		}
	}
	
	public enum MatchType {
		Trios,
		Duos,
		Arena
	}
}