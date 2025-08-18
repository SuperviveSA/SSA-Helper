using Microsoft.Extensions.DependencyInjection;

using Shared.Schemas;
using Shared.Schemas.Supervive.Public;

namespace Shared.Services {
	public interface ITournamentService {
		public Dictionary<string, int>                   CalculateTeamPoints(PublicMatchData[][]     matchesData);
		public int                                       CalculateSinglePlayerPoints(PublicMatchData playerMatchData);
		public IReadOnlyDictionary<string, RosterResult> AggregateRosters(PublicMatchData[][]        matchesData, int minOverlap = 2);
	}

	public class TournamentService :ITournamentService {
		private readonly Dictionary<int, int> positionPoints = new() {
			{ 1, 20 },
			{ 2, 15 },
			{ 3, 12 },
			{ 4, 10 },
			{ 5, 8 },
			{ 6, 6 },
			{ 7, 4 },
			{ 8, 2 },
			{ 9, 1 },
			{ 10, 0 },
			{ 11, 0 },
			{ 12, 0 }
		};

		public static void ConfigureService(IServiceCollection services) {
			services.AddScoped<ITournamentService, TournamentService>();
		}

		public Dictionary<string, int> CalculateTeamPoints(PublicMatchData[][] matchesData) {
			Dictionary<string, int> teamPoints = [];

			foreach (PublicMatchData[] match in matchesData) {
				foreach (IGrouping<int, PublicMatchData> teamGroup in match.GroupBy(p => p.TeamId)) {
					IEnumerable<string> sortedPlayerIds = teamGroup.Select(p => p.PlayerIdEncoded).OrderBy(id => id);
					string              rosterKey       = string.Join('|', sortedPlayerIds);

					// Placement is the same for every player of a team in a match
					int placement    = teamGroup.First().Placement;
					int placementPts = this.positionPoints.GetValueOrDefault(placement, 0);

					// Ensure team exists, then accumulate placement + player points
					teamPoints.TryAdd(rosterKey, 0);
					teamPoints[rosterKey] += placementPts;

					teamPoints[rosterKey] += teamGroup.Sum(this.CalculateSinglePlayerPoints);
				}
			}

			return teamPoints;
		}

		public int CalculateSinglePlayerPoints(PublicMatchData playerMatchData) {
			return playerMatchData.Stats.Kills;
		}

		public IReadOnlyDictionary<string, RosterResult> AggregateRosters(PublicMatchData[][] matchesData, int minOverlap = 2) {
			List<RosterResult>               rosters = [];
			Dictionary<string, RosterResult> byId    = new();

			foreach (PublicMatchData[] match in matchesData) {
				foreach (IGrouping<int, PublicMatchData> teamGroup in match.GroupBy(p => p.TeamId)) {
					string[] memberIds = teamGroup.Select(p => p.PlayerIdEncoded).OrderBy(id => id).ToArray();

					RosterResult? roster = FindBestRosterMatch(rosters, memberIds, minOverlap);
					if (roster is null) {
						string newRosterId = string.Join('|', memberIds);
						roster = new RosterResult {
							RosterId = newRosterId
						};
						roster.CanonicalMemberIds.UnionWith(memberIds);
						rosters.Add(roster);
						byId[newRosterId] = roster;
					}

					int placement    = teamGroup.First().Placement;
					int placementPts = this.positionPoints.GetValueOrDefault(placement, 0);
					roster.Placements.Add(placement);
					roster.Points += placementPts + teamGroup.Sum(this.CalculateSinglePlayerPoints);

					foreach (PublicMatchData p in teamGroup) {
						if (!roster.Players.TryGetValue(p.PlayerIdEncoded, out PublicMatchData? totals)) {
							totals                            = p;
							roster.Players[p.PlayerIdEncoded] = totals;
						}

						AddStats(totals.Stats, p.Stats);
					}
				}
			}

			return byId;
		}

		private static RosterResult? FindBestRosterMatch(IEnumerable<RosterResult> rosters, string[] memberIdsSorted, int minOverlap) {
			RosterResult? best      = null;
			int           bestMatch = 0;

			HashSet<string> current = new(memberIdsSorted);

			foreach (RosterResult roster in rosters) {
				int overlap = roster.CanonicalMemberIds.Count(id => current.Contains(id));

				// Prefer highest overlap; if exact set, break ties in favor of that
				if (overlap <= bestMatch && (overlap != bestMatch
										  || overlap != roster.CanonicalMemberIds.Count
										  || !roster.CanonicalMemberIds.SetEquals(current)))
					continue;
				best      = roster;
				bestMatch = overlap;
			}

			if (best is not null && bestMatch >= minOverlap) return best;
			return null;
		}

		private static void AddStats(PublicPlayerStats dst, PublicPlayerStats add) {
			dst.Kills                    += add.Kills;
			dst.Deaths                   += add.Deaths;
			dst.Assists                  += add.Assists;
			dst.HealingGiven             += add.HealingGiven;
			dst.HealingGivenSelf         += add.HealingGivenSelf;
			dst.HeroEffectiveDamageDone  += add.HeroEffectiveDamageDone;
			dst.HeroEffectiveDamageTaken += add.HeroEffectiveDamageTaken;
		}
	}
}