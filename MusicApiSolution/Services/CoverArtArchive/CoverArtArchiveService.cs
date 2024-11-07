using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ArtistInfo.Api.Services.CoverArtArchive
{
    /// <summary>
    /// Service for interacting with the Cover Art Archive API to fetch cover art information for release groups.
    /// </summary>
    public class CoverArtArchiveService : ICoverArtArchiveService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CoverArtArchiveService> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CoverArtArchiveService"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client used for making requests to the Cover Art Archive API.</param>
        /// <param name="logger">The logger used for logging errors and warnings.</param>
        public CoverArtArchiveService(HttpClient httpClient, ILogger<CoverArtArchiveService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Fetches cover art URLs for a list of release group IDs concurrently.
        /// </summary>
        /// <param name="groupIds">The list of release group IDs to fetch cover art for.</param>
        /// <returns>A collection of cover art URLs.</returns>
        public async Task<IEnumerable<string>> FetchCoverArtsAsync(List<string> groupIds)
        {
            var coverArtTasks = groupIds.Select(FetchCoverArtAsync).ToList();
            var coverArts = await Task.WhenAll(coverArtTasks);
            return coverArts;
        }

        /// <summary>
        /// Fetches the cover art URL for a specific release group ID.
        /// </summary>
        /// <param name="groupId">The release group ID to fetch cover art for.</param>
        /// <returns>The cover art URL, or null if not found or an error occurs.</returns>
        public async Task<string> FetchCoverArtAsync(string groupId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://coverartarchive.org/release-group/{groupId}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var json = JObject.Parse(responseContent);
                var imageUrl = json["images"]?.First?["image"]?.ToString();

                return imageUrl ?? string.Empty;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error fetching cover art for release group {GroupId}", groupId);
                return null;
            }
            catch (IndexOutOfRangeException ex)
            {
                _logger.LogWarning(ex, "No cover art found for release group {GroupId}", groupId);
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error parsing JSON for release group {GroupId}", groupId);
                return null;
            }
        }
    }
}
