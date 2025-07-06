using System.Text.Json.Serialization;

namespace Shared.Schemas.Supervive {
	public record DataResponse<T> where T : SuperviveEntity {
		[JsonPropertyName("data")]
		public required T[] Data { get; init; }
		
		[JsonPropertyName("meta")]
		public required Pagination Meta { get; init; }
	}
	
	public record SuperviveEntity { }
}
