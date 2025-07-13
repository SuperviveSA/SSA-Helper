using Microsoft.Extensions.DependencyInjection;

using Shared.Schemas.Supervive.Public;

namespace Shared.Services {
	public interface ITournamentService {
		public PublicMatchData[]    SumPlayerStats(PublicMatchData[][]                    matchesData);
		public Dictionary<int, int> CalculateTeamPoints(PublicMatchData[][]       matchesData);
		public int                  CalculateSinglePlayerPoints(PublicMatchData playerMatchData);
	}

	public class TournamentService(ISuperviveService supervive) :ITournamentService {
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
		
		public PublicMatchData[] SumPlayerStats(PublicMatchData[][] matchesData) {
			Dictionary<string, PublicMatchData> data = [];
			foreach (PublicMatchData[] matchData in matchesData)
			foreach (PublicMatchData playerData in matchData) {
				if (!data.TryGetValue(playerData.PlayerIdEncoded, out PublicMatchData? currentData)) {
					data.Add(playerData.PlayerIdEncoded, playerData);
					continue;
				}

				data[playerData.PlayerIdEncoded] = playerData with {
					Stats = new PublicPlayerStats {
						Kills                    = playerData.Stats.Kills                    + currentData.Stats.Kills,
						Deaths                   = playerData.Stats.Deaths                   + currentData.Stats.Deaths,
						Assists                  = playerData.Stats.Assists                  + currentData.Stats.Assists,
						HealingGiven             = playerData.Stats.HealingGiven             + currentData.Stats.HealingGiven,
						HealingGivenSelf         = playerData.Stats.HealingGivenSelf         + currentData.Stats.HealingGivenSelf,
						HeroEffectiveDamageDone  = playerData.Stats.HeroEffectiveDamageDone  + currentData.Stats.HeroEffectiveDamageDone,
						HeroEffectiveDamageTaken = playerData.Stats.HeroEffectiveDamageTaken + currentData.Stats.HeroEffectiveDamageTaken
					}
				};
			}

			return data.Values.ToArray();
		}

		public Dictionary<int, int> CalculateTeamPoints(PublicMatchData[][] matchesData) {
			Dictionary<int, int> teamPoints = [];

			foreach (PublicMatchData[] match in matchesData) {
				foreach (PublicMatchData playerMatchData in match.DistinctBy(m => m.TeamId)) {
					teamPoints[playerMatchData.TeamId] = this.positionPoints[playerMatchData.Placement];
				}
				
				foreach (PublicMatchData playerMatchData in match)
					teamPoints[playerMatchData.TeamId] += this.CalculateSinglePlayerPoints(playerMatchData);	
			}

			return teamPoints;
		}

		public int CalculateSinglePlayerPoints(PublicMatchData playerMatchData) {
			return playerMatchData.Stats.Kills;
		}
	}
}