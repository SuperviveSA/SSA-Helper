using System.Text.Json.Serialization;

namespace Shared.Schemas.Supervive.Private {
	public record PrivatePlayerData {
		[JsonPropertyName("platform")]
		public required string Platform { get; init; }
		
		[JsonPropertyName("uniqueDisplayName")]
		public required string UniqueDisplayName { get; init; }
		
		[JsonPropertyName("displayName")]
		public required string DisplayName { get; init; }
		
		[JsonPropertyName("userId")]
		public required string UserId { get; init; }
		
		[JsonPropertyName("source")]
		public required string Source { get; init; }
	}
}
