using System.Text.Json.Serialization;

namespace Shared.Schemas.Supervive {
	public record Pagination {
		[JsonPropertyName("current_page")]
		public int CurrentPage { get; init; }
		
		[JsonPropertyName("last_page")]
		public int LastPage { get; init; }
		
		[JsonPropertyName("per_page")]
		public int PerPage { get; init; }
		
		[JsonPropertyName("total")]
		public int Total { get; init; }
	}
}
