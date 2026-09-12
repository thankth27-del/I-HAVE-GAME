using I_HAVE_GAME.Models.Rawg;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace I_HAVE_GAME.Services
{
    /// <summary>
    /// Implementation of RAWG API service using HttpClient
    /// </summary>
    public class RawgService : IRawgService
    {
        private readonly HttpClient _httpClient;
        private readonly RawgOptions _options;
        private readonly ILogger<RawgService> _logger;

        public RawgService(HttpClient httpClient, IOptions<RawgOptions> options, ILogger<RawgService> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
        }

        public bool IsConfigured()
        {
            return !string.IsNullOrWhiteSpace(_options.ApiKey);
        }

        public async Task<RawgGameListResponse?> SearchGamesAsync(
            string? genre = null,
            string? platforms = null,
            string? search = null,
            string? ordering = null,
            int pageSize = 20,
            int page = 1,
            CancellationToken cancellationToken = default)
        {
            return await SearchGamesAsync(genre, platforms, null, search, ordering, pageSize, page, cancellationToken);
        }

        public async Task<RawgGameListResponse?> SearchGamesAsync(
            string? genre,
            string? platforms,
            string? released,
            string? search,
            string? ordering,
            int pageSize,
            int page,
            CancellationToken cancellationToken = default)
        {
            if (!IsConfigured())
            {
                _logger.LogError("RAWG API key is not configured.");
                return null;
            }

            try
            {
                var queryParams = new List<string>
                {
                    $"key={Uri.EscapeDataString(_options.ApiKey)}",
                    $"page_size={Math.Min(pageSize, 40)}",
                    $"page={Math.Max(page, 1)}"
                };

                if (!string.IsNullOrWhiteSpace(genre))
                    queryParams.Add($"genres={Uri.EscapeDataString(genre)}");

                if (!string.IsNullOrWhiteSpace(platforms))
                    queryParams.Add($"platforms={Uri.EscapeDataString(platforms)}");

                if (!string.IsNullOrWhiteSpace(released))
                    queryParams.Add($"dates={Uri.EscapeDataString(released)}");

                if (!string.IsNullOrWhiteSpace(search))
                    queryParams.Add($"search={Uri.EscapeDataString(search)}");

                if (!string.IsNullOrWhiteSpace(ordering))
                    queryParams.Add($"ordering={Uri.EscapeDataString(ordering)}");

                var url = $"{_options.BaseUrl}games?{string.Join("&", queryParams)}";

                _logger.LogInformation("Calling RAWG API: {Url}", url.Replace(_options.ApiKey, "[REDACTED]"));

                var response = await _httpClient.GetAsync(url, cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                {
                    _logger.LogWarning("RAWG API rate limit exceeded (429).");
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("RAWG API returned status {StatusCode}", response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<RawgGameListResponse>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                return result;
            }
            catch (TaskCanceledException)
            {
                _logger.LogWarning("RAWG API request timed out.");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling RAWG API: {Message}", ex.Message);
                return null;
            }
        }

        public async Task<List<RawgGenre>?> GetGenresAsync(CancellationToken cancellationToken = default)
        {
            if (!IsConfigured())
            {
                _logger.LogError("RAWG API key is not configured.");
                return null;
            }

            try
            {
                var url = $"{_options.BaseUrl}genres?key={Uri.EscapeDataString(_options.ApiKey)}";

                var response = await _httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to get genres from RAWG API: {StatusCode}", response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<RawgGenreListResponse>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                return result?.Results ?? new List<RawgGenre>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting genres from RAWG API: {Message}", ex.Message);
                return null;
            }
        }

        public async Task<List<RawgPlatform>?> GetPlatformsAsync(CancellationToken cancellationToken = default)
        {
            if (!IsConfigured())
            {
                _logger.LogError("RAWG API key is not configured.");
                return null;
            }

            try
            {
                var url = $"{_options.BaseUrl}platforms?key={Uri.EscapeDataString(_options.ApiKey)}";

                var response = await _httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to get platforms from RAWG API: {StatusCode}", response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<RawgPlatformListResponse>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                return result?.Results ?? new List<RawgPlatform>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting platforms from RAWG API: {Message}", ex.Message);
                return null;
            }
        }

        public async Task<RawgGame?> GetGameAsync(int gameId, CancellationToken cancellationToken = default)
        {
            if (!IsConfigured())
            {
                _logger.LogError("RAWG API key is not configured.");
                return null;
            }

            try
            {
                var url = $"{_options.BaseUrl}games/{gameId}?key={Uri.EscapeDataString(_options.ApiKey)}";

                var response = await _httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to get game {GameId} from RAWG API: {StatusCode}", gameId, response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                var result = JsonSerializer.Deserialize<RawgGame>(content, new JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting game {GameId} from RAWG API: {Message}", gameId, ex.Message);
                return null;
            }
        }
    }
}
