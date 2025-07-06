using System.Text.Json.Serialization;

namespace Shared.Schemas.Supervive.Public {
	public record PublicMatchData :SuperviveEntity {
		[JsonPropertyName("character_level")]
		public int CharacterLevel { get; init; }

		[JsonPropertyName("hero")]
		public required PublicHeroData Hero { get; init; }

		[JsonPropertyName("hero_asset_id")]
		public required string HeroAssetId { get; init; }

		[JsonPropertyName("is_ranked")]
		public bool IsRanked { get; init; }

		[JsonPropertyName("match_end")]
		public DateTime MatchEnd { get; init; }

		[JsonPropertyName("placement")]
		public int Placement { get; init; }

		[JsonPropertyName("player")]
		public required PublicPlayerData Player { get; init; }

		[JsonPropertyName("player_id_encoded")]
		public required string PlayerIdEncoded { get; init; }

		[JsonPropertyName("stats")]
		public required PublicPlayerStats Stats { get; init; }

		[JsonPropertyName("survival_duration")]
		public double SurvivalDuration { get; init; }

		[JsonPropertyName("team_id")]
		public int TeamId { get; init; }
	}
}