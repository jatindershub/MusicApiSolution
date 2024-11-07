using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace ArtistInfo.Api.Services.CoverArtArchive
{
    public class CoverArtArchiveService : ICoverArtArchiveService
    {
        private readonly HttpClient _httpClient;

        public CoverArtArchiveService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>> GetCoverArtsAsync(List<string> releaseGroupIds)
        {
            var coverArtTasks = releaseGroupIds.Select(id => FetchCoverArtAsync(id)).ToList();
            var coverArts = await Task.WhenAll(coverArtTasks);
            return coverArts.ToList();
        }

        public async Task<string> FetchCoverArtAsync(string releaseGroupId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"https://coverartarchive.org/release-group/{releaseGroupId}");
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var jsonDocument = JsonDocument.Parse(responseContent);
                var imageUrl = jsonDocument.RootElement.GetProperty("images")[0].GetProperty("image").GetString();

                return imageUrl;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error fetching cover art for release group {releaseGroupId}: {ex.Message}");
                return null; // Return null if the cover art couldn't be fetched
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine($"No cover art found for release group {releaseGroupId}");
                return null; // Return null if no cover art is found
            }
        }
    }
}
