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

        public async Task<string> GetCoverArtAsync(string releaseGroupId)
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
                // If no cover art is found, return null
            }

            return null;
        }
    }
}
