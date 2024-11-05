using Newtonsoft.Json.Linq;

namespace ArtistInfo.Api.Services.MusicBrainz
{
    public class MusicBrainzService : IMusicBrainzService
    {
        private readonly HttpClient _httpClient;

        public MusicBrainzService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "MusicApi"); // todo: find ud af hvad der giver bedst mening her
        }

        public async Task<JObject> GetArtistAsync(string mbid)
        {
            var url = $"https://musicbrainz.org/ws/2/artist/{mbid}?fmt=json&inc=url-rels+release-groups";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JObject.Parse(content);
        }
    }
}
