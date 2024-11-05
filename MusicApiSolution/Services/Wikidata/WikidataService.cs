using Newtonsoft.Json.Linq;

namespace ArtistInfo.Api.Services.Wikidata
{
    public class WikidataService : IWikidataService
    {
        private readonly HttpClient _httpClient;

        public WikidataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetWikipediaTitleAsync(string wikidataId)
        {
            var wikidataApiUrl = $"https://www.wikidata.org/w/api.php?action=wbgetentities&ids={wikidataId}&format=json&props=sitelinks";
            var response = await _httpClient.GetAsync(wikidataApiUrl);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var wikidataJson = JObject.Parse(content);

            return wikidataJson["entities"]?[wikidataId]?["sitelinks"]?["enwiki"]?["title"]?.ToString();

        }
    }
}
