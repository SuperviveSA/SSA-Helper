using System.Text.Json.Serialization;

namespace Shared.Schemas.Supervive.Private {
	public record PrivateHeroData {
		[JsonPropertyName("asset_id")]
		public required string AssetId { get; init; }
		
		[JsonPropertyName("head_image_url")]
		public required string HeadImageUrl { get; init; }
	}
}
