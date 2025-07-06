using System.Text.Json.Serialization;

namespace Shared.Schemas.Supervive.Public {
	public record PublicPlayerData {
		[JsonPropertyName("display_name")]
		public string? DisplayName { get; init; }
		
		[JsonPropertyName("unique_display_name")]
		public string? UniqueDisplayName { get; init; }
	}
}
