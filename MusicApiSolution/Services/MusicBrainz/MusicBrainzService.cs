using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ArtistInfo.Api.Services.MusicBrainz
{
    public class MusicBrainzService : IMusicBrainzService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MusicBrainzService> _logger;

        public MusicBrainzService(HttpClient httpClient, ILogger<MusicBrainzService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "MusicApi");
        }

        public async Task<JObject> GetArtistAsync(string mbid)
        {
            if (string.IsNullOrEmpty(mbid))
            {
                _logger.LogError("GetArtistAsync: MBID is null or empty");
                throw new ArgumentException("MBID cannot be null or empty", nameof(mbid));
            }

            try
            {
                var url = $"https://musicbrainz.org/ws/2/artist/{mbid}?fmt=json&inc=url-rels+release-groups";
                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("GetArtistAsync: Request to MusicBrainz failed with status code {StatusCode}, reason: {ReasonPhrase}", response.StatusCode, response.ReasonPhrase);
                    return null; // or consider throwing an exception if this fits your scenario
                }

                var content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(content))
                {
                    _logger.LogWarning("GetArtistAsync: Received empty response from MusicBrainz for MBID {MBID}", mbid);
                    return null;
                }

                return JObject.Parse(content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "GetArtistAsync: Error occurred while making request to MusicBrainz for MBID {MBID}", mbid);
                throw; // Re-throwing after logging to ensure the caller is aware of the failure
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "GetArtistAsync: Failed to parse JSON response from MusicBrainz for MBID {MBID}", mbid);
                throw new InvalidOperationException("Invalid JSON response received from MusicBrainz", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetArtistAsync: Unexpected error occurred for MBID {MBID}", mbid);
                throw; // Re-throw to ensure unexpected issues are surfaced
            }
        }
    }
}
