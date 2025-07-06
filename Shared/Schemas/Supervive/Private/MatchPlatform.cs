using System.Text.Json.Serialization;

namespace Shared.Schemas.Supervive.Private {
	public record MatchPlatform {
		[JsonPropertyName("code")]
		public required string Code { get; init; }
		
		[JsonPropertyName("id")]
		public required int Id { get; init; }
	}
}
