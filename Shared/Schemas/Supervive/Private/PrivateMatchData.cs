using System.Text.Json.Serialization;

namespace Shared.Schemas.Supervive.Private {
	public record PrivateMatchData : SuperviveEntity{
		[JsonPropertyName("character_level")]
		public int CharacterLevel { get; init; }
		
		[JsonPropertyName("hero")]
		public required PrivateHeroData Hero { get; init; }
		
		[JsonPropertyName("hero_asset_id")]
		public required string HeroAssetId { get; init; }
		
		[JsonPropertyName("is_ranked")]
		public bool IsRanked { get; init; }
		
		[JsonPropertyName("match_end")]
		public DateTime MatchEnd { get; init; }
		
		[JsonPropertyName("placement")]
		public int Placement { get; init; }
		
		[JsonPropertyName("platform")]
		public required MatchPlatform Platform { get; init; }
		
		[JsonPropertyName("player_id_encoded")]
		public required string PlayerIdEncoded { get; init; }
		
		[JsonPropertyName("player_id")]
		public required string PlayerId { get; init; }
		
		[JsonPropertyName("stats")]
		public required PrivatePlayerStats Stats { get; init; }
		
		[JsonPropertyName("survival_duration")]
		public double SurvivalDuration { get; init; }
		
		[JsonPropertyName("team_id")]
		public int TeamId { get; init; }
		
		[JsonPropertyName("id")]
		public int Id { get; init; }
		
		[JsonPropertyName("match_id")]
		public required string MatchId { get; init; }
		
		[JsonPropertyName("queue_id")]
		public required string QueueId { get; init; }
		
		[JsonPropertyName("party_id")]
		public string? PartyId { get; init; }
		
		[JsonPropertyName("created_at")]
		public DateTime CreatedAt { get; init; }
		
		[JsonPropertyName("match_start")]
		public DateTime MatchStart { get; init; }
	}
}
