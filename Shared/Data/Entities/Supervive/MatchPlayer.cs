using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Data.Entities.Supervive {
	[Index(nameof(TeamId))]
	[Index(nameof(Hero))]
	[Index(nameof(CreatedAt))]
	[Index(nameof(DeletedAt))]
	[PrimaryKey(nameof(MatchId), nameof(PlayerIdEncoded))]
	[Table("match_player")]
	public class MatchPlayer :IEntityTypeConfiguration<MatchPlayer> {

		#region Ids

		[Column("match_id")]
		[JsonPropertyName("match_id")]
		public required string MatchId { get; set; }
		
		[Column("player_id_encoded")]
		[JsonPropertyName("player_id_encoded")]
		public required string PlayerIdEncoded { get; set; }
		
		[Column("team_id")]
		[JsonPropertyName("team_id")]
		public int TeamId { get; set; }

		#endregion

		#region General Stats

		[Column("hero")]
		[JsonPropertyName("hero")]
		public required string Hero { get; set; }
		
		[Column("survival_duration")]
		[JsonPropertyName("survival_duration")]
		public double SurvivalDuration { get; set; }
		
		[Column("kills")]
		[JsonPropertyName("kills")]
		public int Kills { get; set; }
		
		[Column("deaths")]
		[JsonPropertyName("deaths")]
		public int Deaths { get; set; }
		
		[Column("assists")]
		[JsonPropertyName("assists")]
		public int Assists { get; set; }
		#endregion

		#region Gameplay Stats
		
		[JsonPropertyName("healing_given")]
		public double HealingGiven { get; set; }
		
		[JsonPropertyName("healing_given_self")]
		public double HealingGivenSelf { get; set; }
		
		[JsonPropertyName("hero_effective_damage_done")]
		public double HeroEffectiveDamageDone { get; set; }
		
		[JsonPropertyName("hero_effective_damage_taken")]
		public double HeroEffectiveDamageTaken { get; set; }
		
		#endregion

		#region Rating

		[Column("rating_delta")]
		[JsonPropertyName("rating_delta")]
		public float? Delta { get; set; }
		
		[Column("rating")]
		[JsonPropertyName("rating")]
		public float? Rating { get; set; }
		
		#endregion

		#region Entity stats
		
		[Column("created_at")]
		[JsonPropertyName("created_at")]
		public DateTime CreatedAt { get; set; }
		
		[Column("deleted_at")]
		[JsonPropertyName("deleted_at")]
		public DateTime? DeletedAt { get; set; }

		#endregion

		#region Relationships

		[JsonIgnore, ForeignKey(nameof(MatchId))]
		[InverseProperty(nameof(Match.Players))]
		public Match? Match { get; set; }
		
		[JsonIgnore, ForeignKey(nameof(PlayerIdEncoded))]
		[InverseProperty(nameof(Entities.Player.MatchesPlayed))]
		public Player? Player { get; set; }

		#endregion
		
		public void Configure(EntityTypeBuilder<MatchPlayer> builder) {
			builder.Property(mp => mp.CreatedAt)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP");
		}
	}
}