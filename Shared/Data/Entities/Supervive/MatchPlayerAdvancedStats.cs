using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Shared.Data.Entities.Supervive {
	[Index(nameof(CreatedAt))]
	[PrimaryKey(nameof(MatchId), nameof(PlayerId))]
	[Table("match_player_advanced_stats")]
	public class MatchPlayerAdvancedStats :IEntityTypeConfiguration<MatchPlayerAdvancedStats> {
		#region Ids

		[Column("match_id")]
		[JsonPropertyName("match_id")]
		public required string MatchId { get; set; }

		[Column("player_id")]
		[JsonPropertyName("player_id")]
		public required string PlayerId { get; set; }

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

		[Column("ressurects")]
		[JsonPropertyName("ressurects")]
		public int Ressurects { get; set; }

		[Column("revived")]
		[JsonPropertyName("revived")]
		public int Revived { get; set; }

		[Column("knocks")]
		[JsonPropertyName("knocks")]
		public int Knocks { get; set; }

		[Column("knocked")]
		[JsonPropertyName("knocked")]
		public int Knocked { get; set; }

		[Column("max_kill_streak")]
		[JsonPropertyName("max_kill_streak")]
		public int MaxKillStreak { get; set; }

		[Column("max_knock_streak")]
		[JsonPropertyName("max_knock_streak")]
		public int MaxKnockStreak { get; set; }

		#endregion

		#region Gameplay Stats

		[Column("creep_kills")]
		[JsonPropertyName("creep_kills")]
		public int CreepKills { get; set; }

		[Column("gold_from_enemies")]
		[JsonPropertyName("gold_from_enemies")]
		public int GoldFromEnemies { get; set; }

		[Column("gold_from_monsters")]
		[JsonPropertyName("gold_from_monsters")]
		public int GoldFromMonsters { get; set; }

		// Healing
		[Column("healing_given")]
		[JsonPropertyName("healing_given")]
		public double HealingGiven { get; set; }

		[Column("healing_given_self")]
		[JsonPropertyName("healing_given_self")]
		public double HealingGivenSelf { get; set; }

		[Column("healing_received")]
		[JsonPropertyName("healing_received")]
		public double HealingReceived { get; set; }

		// Damage dealt
		[Column("damage_done")]
		[JsonPropertyName("damage_done")]
		public double DamageDone { get; set; }

		[Column("effective_damage_done")]
		[JsonPropertyName("effective_damage_done")]
		public double EffectiveDamageDone { get; set; }

		[Column("hero_damage_done")]
		[JsonPropertyName("hero_damage_done")]
		public double HeroDamageDone { get; set; }

		[Column("hero_effective_damage_done")]
		[JsonPropertyName("hero_effective_damage_done")]
		public double HeroEffectiveDamageDone { get; set; }

		// Damage taken
		[Column("damage_taken")]
		[JsonPropertyName("damage_taken")]
		public double DamageTaken { get; set; }

		[Column("effective_damage_taken")]
		[JsonPropertyName("effective_damage_taken")]
		public double EffectiveDamageTaken { get; set; }

		[Column("hero_damage_taken")]
		[JsonPropertyName("hero_damage_taken")]
		public double HeroDamageTaken { get; set; }

		[Column("hero_effective_damage_taken")]
		[JsonPropertyName("hero_effective_damage_taken")]
		public double HeroEffectiveDamageTaken { get; set; }

		[Column("shield_mitigated_damage")]
		[JsonPropertyName("shield_mitigated_damage")]
		public double ShieldMitigatedDamage { get; set; }

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
		[InverseProperty(nameof(Match.PlayersAdvancedStats))]
		public Match? Match { get; set; }
		
		[JsonIgnore, ForeignKey(nameof(PlayerId))]
		[InverseProperty(nameof(Player.MatchesPlayedAdvancedStats))]
		public Player? Player { get; set; }

		#endregion
		
		public void Configure(EntityTypeBuilder<MatchPlayerAdvancedStats> builder) {
			builder.Property(mpas => mpas.CreatedAt)
				   .HasDefaultValueSql("CURRENT_TIMESTAMP");
		}
	}
}