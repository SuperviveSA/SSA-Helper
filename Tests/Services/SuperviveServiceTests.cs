using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

using Shared.Schemas.Supervive;
using Shared.Schemas.Supervive.Private;
using Shared.Schemas.Supervive.Public;
using Shared.Services;

namespace Tests.Services {
	public class SuperviveServiceTests {
		readonly ISuperviveService service;

		public SuperviveServiceTests() {
			IServiceCollection services = new ServiceCollection();
			services.AddLogging(b => b.AddConsole());

			services.AddSingleton<IDistributedCache, MemoryDistributedCache>();
			SuperviveService.ConfigureService(services);
			
			IServiceProvider provider = services.BuildServiceProvider();
			this.service = provider.GetRequiredService<ISuperviveService>();
		}

		[Fact(DisplayName = "When getting match by id -> returns correct data")]
		public async Task WhenGettingMatch_ReturnsCorrectData() {
			// Arrange
			const string platform = "steam";
			const string matchId  = "0da7a2e4-71d3-466d-bbf8-219831bb2625";

			// Act
			PublicMatchData[] match = await this.service.GetMatch(platform, matchId);

			// Assert
			Assert.True(match.Length == 8, "Incorrect data length");
		}

		[Fact(DisplayName = "When fetching player data -> returns valid status code")]
		public async Task WhenFetchingPlayerData_ReturnsValidStatusCode() {
			// Arrange
			const string platform = "steam";
			const string playerId = "d1f0f10bfcd0483da70f92ef94a478be";

			// Act
			GenericResponse data = await this.service.FetchNewPlayerMatches(platform, playerId);

			// Assert
			Assert.NotNull(data);
			Assert.NotNull(data.Message);
		}

		[Fact(DisplayName = "When checking existing player -> returns true")]
		public async Task WhenCheckingExistingPlayer_ReturnsTrue() {
			// Arrange
			const string platform          = "steam";
			const string uniqueDisplayName = "hanake#0000";

			// Act
			bool data = await this.service.CheckPlayerExists(platform, uniqueDisplayName);

			// Assert
			Assert.True(data, "response is not true");
		}

		[Fact(DisplayName = "When checking not existing player -> returns false")]
		public async Task WhenCheckingNotExistingPlayer_ReturnsFalse() {
			// Arrange
			const string platform          = "steam";
			const string uniqueDisplayName = "dnaiwbdiiy";

			// Act
			bool data = await this.service.CheckPlayerExists(platform, uniqueDisplayName);

			// Assert
			Assert.False(data, "response is true");
		}

		[Fact(DisplayName = "When searching player -> returns correct response")]
		public async Task WhenSearchingPlayer_ReturnsCorrectResponse() {
			// Arrange
			const string uniqueDisplayName = "hanake#0000";
			PrivatePlayerData[] correctResponse = [
				new() {
					DisplayName       = "Hanake0",
					Platform          = "steam",
					UniqueDisplayName = "hanake#0000",
					UserId            = "d1f0f10bfcd0483da70f92ef94a478be",
					Source            = "db"
				}
			];

			// Act
			PrivatePlayerData[] result = await this.service.SearchPlayers(uniqueDisplayName);

			// Assert
			Assert.Equal(correctResponse, result);
		}
	}
}