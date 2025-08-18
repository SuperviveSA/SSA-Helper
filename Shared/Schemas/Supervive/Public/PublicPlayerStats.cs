using System.Text.Json.Serialization;

namespace Shared.Schemas.Supervive.Public {
	public record PublicPlayerStats {
		[JsonPropertyName("Kills")]
		public int Kills { get; set; }
		
		[JsonPropertyName("Deaths")]
		public int Deaths { get; set; }
		
		[JsonPropertyName("Assists")]
		public int Assists { get; set; }
		
		[JsonPropertyName("HealingGiven")]
		public double HealingGiven { get; set; }
		
		[JsonPropertyName("HealingGivenSelf")]
		public double HealingGivenSelf { get; set; }
		
		[JsonPropertyName("HeroEffectiveDamageDone")]
		public double HeroEffectiveDamageDone { get; set; }
		
		[JsonPropertyName("HeroEffectiveDamageTaken")]
		public double HeroEffectiveDamageTaken { get; set; }
	}
}
