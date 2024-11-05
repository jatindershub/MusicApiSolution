using Newtonsoft.Json.Linq;

namespace ArtistInfo.Api.Services.Wikipedia
{
    public class WikipediaService : IWikipediaService
    {
        private readonly HttpClient _httpClient;

        public WikipediaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> GetDescriptionAsync(string wikipediaTitle)
        {
            if (string.IsNullOrEmpty(wikipediaTitle))
                return null;

            var encodedTitle = System.Web.HttpUtility.UrlEncode(wikipediaTitle);
            var wikipediaApiUrl = $"https://en.wikipedia.org/w/api.php?action=query&titles={encodedTitle}&prop=extracts&exintro&explaintext&format=json";
            var response = await _httpClient.GetAsync(wikipediaApiUrl);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var wikipediaJson = JObject.Parse(content);

            var page = wikipediaJson["query"]?["pages"]?.First?.First;
            return page?["extract"]?.ToString();
        }
    }
}
