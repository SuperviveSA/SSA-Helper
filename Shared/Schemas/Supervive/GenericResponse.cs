using System.Text.Json.Serialization;

namespace Shared.Schemas.Supervive {
	public record GenericResponse {
		[JsonPropertyName("message")]
		public required string Message { get; init; }
		
		[JsonPropertyName("success")]
		public bool Success { get; init; }
	}
}
