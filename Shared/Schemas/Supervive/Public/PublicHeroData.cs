using System.Text.Json.Serialization;

namespace Shared.Schemas.Supervive.Public {
	public record PublicHeroData {
		[JsonPropertyName("head_image_url")]
		public required string HeadImageUrl { get; init; }
		
		[JsonPropertyName("name")]
		public required string Name { get; init; }
	}
}
