using System.Text.Json.Serialization;

namespace Shared.Schemas.Supervive {
	public record ExistsResponse {
		[JsonPropertyName("exists")]
		public bool Exists { get; init; }	
	}
}
