using System.Net.Http.Json;
using System.Text.Json;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;

using Shared.Schemas.Supervive;
using Shared.Schemas.Supervive.Private;
using Shared.Schemas.Supervive.Public;

namespace Shared.Services {
	public interface ISuperviveService {
		Task<bool>                           CheckPlayerExists(string     platform, string uniqueDisplayName);
		Task<PrivatePlayerData[]>            SearchPlayers(string         query,    bool   useCache = true);
		Task<PublicMatchData[]>              GetMatch(string              platform, string matchId);
		Task<DataResponse<PrivateMatchData>> GetPlayerMatches(string      platform, string playerId, int page = 1);
		Task<GenericResponse>                FetchNewPlayerMatches(string platform, string playerId);
	}

	public partial class SuperviveService(IDistributedCache cache, HttpClient client, IHttpClientFactory httpFactory) :ISuperviveService {
		#region ConfigureService

		private static readonly Uri Url = new("https://supervive.op.gg/");

		public static void ConfigureService(IServiceCollection services) {
			services.AddHttpClient<ISuperviveService, SuperviveService>(client => {
						 client.BaseAddress = Url;
					 })
					.AddStandardResilienceHandler(options => {
						 options.Retry.Name                   = "Supervive-Retry";
						 options.Retry.MaxRetryAttempts       = 15;
						 options.Retry.ShouldRetryAfterHeader = true;

						 // Disable circuit breaker
						 options.CircuitBreaker.ShouldHandle     = _ => ValueTask.FromResult(false);
						 options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(240);
						 options.CircuitBreaker.FailureRatio     = 1;

						 options.AttemptTimeout.Name    = "Supervive-AttemptTimeout";
						 options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(60);

						 options.TotalRequestTimeout.Name    = "Supervive-TotalRequestTimeout";
						 options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(180);
					 });

			services.AddHttpClient("Supervive‐NoRetry", client => {
				client.Timeout     = TimeSpan.FromSeconds(15);
				client.BaseAddress = Url;
			});
		}

		#endregion

		public async Task<bool> CheckPlayerExists(string platform, string uniqueDisplayName) {
			HttpResponseMessage res = await client.GetAsync(QueryHelpers.AddQueryString("api/players/check", new Dictionary<string, string?> {
				{ "platform", platform },
				{ "uniqueDisplayName", uniqueDisplayName }
			}));

			res.EnsureSuccessStatusCode();

			return (await res.Content.ReadFromJsonAsync<ExistsResponse>())?.Exists
				?? throw new NullReferenceException("data is null");
		}

		public async Task<PrivatePlayerData[]> SearchPlayers(string query, bool useCache = true) {
			string               key    = $"search:{query}";
			PrivatePlayerData[]? cached = JsonSerializer.Deserialize<PrivatePlayerData[]>(await cache.GetStringAsync(key) ?? "null");

			if (cached is not null) return cached;

			HttpResponseMessage res = await client.GetAsync(QueryHelpers.AddQueryString("api/players/search", new Dictionary<string, string?> {
				{ "query", query }
			}));

			res.EnsureSuccessStatusCode();

			PrivatePlayerData[] data = await res.Content.ReadFromJsonAsync<PrivatePlayerData[]>()
									?? throw new NullReferenceException("data is null");

			await cache.SetStringAsync(key, JsonSerializer.Serialize(data), new DistributedCacheEntryOptions {
				AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
			});

			return data;
		}

		private async Task<string> GetXsrfToken(string platform, string playerId) {
			HttpClient          noRetryClient = httpFactory.CreateClient("Supervive‐NoRetry");
			HttpResponseMessage res           = await noRetryClient.GetAsync($"api/players/{platform}-{playerId.Replace("-", "")}/matches?page=1");

			if (!res.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? cookieHeaders))
				throw new Exception("Could not get cookie headers");

			foreach (string cookie in cookieHeaders) {
				if (!cookie.Contains("XSRF-TOKEN=")) continue;

				Match match = XsrfTokenRegex().Match(cookie);
				if (match.Success) return Uri.UnescapeDataString(match.Groups[1].Value);
			}

			throw new Exception("Could not find correct cookie property");
		}

		public async Task<PublicMatchData[]> GetMatch(string platform, string matchId) {
			string             key    = $"match:{platform}:{matchId}";
			PublicMatchData[]? cached = JsonSerializer.Deserialize<PublicMatchData[]>(await cache.GetStringAsync(key) ?? "null");

			if (cached is not null) return cached;

			HttpResponseMessage res = await client.GetAsync($"api/matches/{platform}-{matchId}");

			res.EnsureSuccessStatusCode();

			PublicMatchData[] data = await res.Content.ReadFromJsonAsync<PublicMatchData[]>()
								  ?? throw new NullReferenceException("data is null");

			await cache.SetStringAsync(key, JsonSerializer.Serialize(data), new DistributedCacheEntryOptions {
				AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7)
			});

			return data;
		}

		public async Task<DataResponse<PrivateMatchData>> GetPlayerMatches(string platform, string playerId, int page = 1) {
			HttpResponseMessage res = await client.GetAsync($"api/players/{platform}-{playerId.Replace("-", "")}/matches?page={page}");

			res.EnsureSuccessStatusCode();

			return await res.Content.ReadFromJsonAsync<DataResponse<PrivateMatchData>>()
				?? throw new NullReferenceException("data is null");
		}

		public async Task<GenericResponse> FetchNewPlayerMatches(string platform, string playerId) {
			string csrfToken = await this.GetXsrfToken(platform, playerId);

			using HttpRequestMessage req = new(HttpMethod.Post, $"api/players/{platform}-{playerId.Replace("-", "")}/matches/fetch");
			req.Headers.Add("X-XSRF-TOKEN", csrfToken);

			HttpClient          noRetryClient = httpFactory.CreateClient("Supervive‐NoRetry");
			HttpResponseMessage res           = await noRetryClient.SendAsync(req);

			if (res.Content.Headers.ContentType?.MediaType?.Contains("text/html") ?? true)
				throw new Exception("Invalid player id or XSRF Token");

			return await res.Content.ReadFromJsonAsync<GenericResponse>()
				?? throw new NullReferenceException("data is null");
		}

		[GeneratedRegex("XSRF-TOKEN=([^;]+)")]
		private static partial Regex XsrfTokenRegex();
	}
}