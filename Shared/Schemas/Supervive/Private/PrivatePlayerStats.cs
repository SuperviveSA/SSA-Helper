using System.Text.Json.Serialization;

namespace Shared.Schemas.Supervive.Private {
	public record PrivatePlayerStats {
		
		#region General Stats

		[JsonPropertyName("Kills")]
		public int? Kills { get; init; }
		
		[JsonPropertyName("Deaths")]
		public int? Deaths { get; init; }
		
		[JsonPropertyName("Assists")]
		public int? Assists { get; init; }
		
		[JsonPropertyName("Resurrected")]
		public int? Resurrected { get; init; }
		
		[JsonPropertyName("Revived")]
		public int? Revived { get; init; }
		
		[JsonPropertyName("Knocks")]
		public int? Knocks { get; init; }
		
		[JsonPropertyName("Knocked")]
		public int? Knocked { get; init; }
		
		[JsonPropertyName("MaxKillStreak")]
		public int? MaxKillStreak { get; init; }
		
		[JsonPropertyName("MaxKnockStreak")]
		public int? MaxKnockStreak { get; init; }
		
		#endregion

		#region Gameplay Stats
		
		[JsonPropertyName("CreepKills")]
		public int? CreepKills { get; init; }
		
		[JsonPropertyName("GoldFromEnemies")]
		public int? GoldFromEnemies { get; init; }
		
		[JsonPropertyName("GoldFromMonsters")]
		public int? GoldFromMonsters { get; init; }
		
		// Healing
		[JsonPropertyName("HealingGiven")]
		public double? HealingGiven { get; init; }
		
		[JsonPropertyName("HealingGivenSelf")]
		public double? HealingGivenSelf { get; init; }
		
		[JsonPropertyName("HealingReceived")]
		public double? HealingReceived { get; init; }
		
		// Damage dealt
		[JsonPropertyName("DamageDone")]
		public double? DamageDone { get; init; }
		
		[JsonPropertyName("EffectiveDamageDone")]
		public double? EffectiveDamageDone { get; init; }
		
		[JsonPropertyName("HeroDamageDone")]
		public double? HeroDamageDone { get; init; }
		
		[JsonPropertyName("HeroEffectiveDamageDone")]
		public double? HeroEffectiveDamageDone { get; init; }
		
		// Damage taken
		[JsonPropertyName("DamageTaken")]
		public double? DamageTaken { get; init; }
		
		[JsonPropertyName("EffectiveDamageTaken")]
		public double? EffectiveDamageTaken { get; init; }
		
		[JsonPropertyName("HeroDamageTaken")]
		public double? HeroDamageTaken { get; init; }
		
		[JsonPropertyName("HeroEffectiveDamageTaken")]
		public double? HeroEffectiveDamageTaken { get; init; }
		
		[JsonPropertyName("ShieldMitigatedDamage")]
		public double? ShieldMitigatedDamage { get; init; }

		#endregion
	}
}
