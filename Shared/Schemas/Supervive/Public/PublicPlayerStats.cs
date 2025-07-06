using System.Text.Json.Serialization;

namespace Shared.Schemas.Supervive.Public {
	public record PublicPlayerStats {
		[JsonPropertyName("Kills")]
		public int Kills { get; init; }
		
		[JsonPropertyName("Deaths")]
		public int Deaths { get; init; }
		
		[JsonPropertyName("Assists")]
		public int Assists { get; init; }
		
		[JsonPropertyName("HealingGiven")]
		public double HealingGiven { get; init; }
		
		[JsonPropertyName("HealingGivenSelf")]
		public double HealingGivenSelf { get; init; }
		
		[JsonPropertyName("HeroEffectiveDamageDone")]
		public double HeroEffectiveDamageDone { get; init; }
		
		[JsonPropertyName("HeroEffectiveDamageTaken")]
		public double HeroEffectiveDamageTaken { get; init; }
	}
}
