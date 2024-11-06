using Newtonsoft.Json.Linq;

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
            var coverArtTasks = releaseGroupIds.Select(async releaseGroupId =>
            {
                var coverArtUrl = $"https://coverartarchive.org/release-group/{releaseGroupId}";
                try
                {
                    var response = await _httpClient.GetAsync(coverArtUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        var coverJson = JObject.Parse(content);
                        return coverJson["images"]?.First?["image"]?.ToString();
                    }
                }
                catch (Exception)
                {
                    // If no cover art is found or an exception is thrown, return null
                }

                return null;
            });

            // Run all cover art requests concurrently and return all results, including nulls
            return (await Task.WhenAll(coverArtTasks)).ToList();
        }
    }
}
