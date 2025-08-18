using Shared.Schemas.Supervive.Public;

namespace Shared.Schemas;

public record RosterResult {
	public required string                              RosterId           { get; set; }
	public          HashSet<string>                     CanonicalMemberIds { get; } = [];
	public          List<int>                           Placements         { get; } = [];
	public          int                                 Points             { get; set; }
	public          Dictionary<string, PublicMatchData> Players            { get; } = new();
}