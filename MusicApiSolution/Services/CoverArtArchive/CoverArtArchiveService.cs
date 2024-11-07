using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ArtistInfo.Api.Services.CoverArtArchive
{
    public class CoverArtArchiveService : ICoverArtArchiveService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<CoverArtArchiveService> _logger;

        public CoverArtArchiveService(HttpClient httpClient, ILogger<CoverArtArchiveService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<IEnumerable<string>> FetchCoverArtsAsync(List<string> groupIds)
        {
            var coverArtTasks = groupIds.Select(FetchCoverArtAsync).ToList();
            var coverArts = await Task.WhenAll(coverArtTasks);
            return coverArts;
        }

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
