using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Logging;

namespace ArtistInfo.Api.Services.Wikidata
{
    public class WikidataService : IWikidataService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WikidataService> _logger;

        public WikidataService(HttpClient httpClient, ILogger<WikidataService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetWikipediaTitleAsync(string wikidataId)
        {
            if (string.IsNullOrEmpty(wikidataId))
            {
                _logger.LogWarning("Wikidata ID is null or empty");
                throw new ArgumentException("Wikidata ID cannot be null or empty", nameof(wikidataId));
            }

            var wikidataApiUrl = $"https://www.wikidata.org/w/api.php?action=wbgetentities&ids={wikidataId}&format=json&props=sitelinks";

            try
            {
                _logger.LogInformation("Requesting Wikipedia title for Wikidata ID: {WikidataId}", wikidataId);
                var response = await _httpClient.GetAsync(wikidataApiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Request to Wikidata API failed with status code: {StatusCode}", response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var wikidataJson = JObject.Parse(content);

                var title = wikidataJson["entities"]?[wikidataId]?["sitelinks"]?["enwiki"]?["title"]?.ToString();

                if (title == null)
                {
                    _logger.LogWarning("Could not find Wikipedia title for Wikidata ID: {WikidataId}", wikidataId);
                }

                return title;
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "An error occurred while making the HTTP request to Wikidata API for ID: {WikidataId}", wikidataId);
                throw; // Consider rethrowing or handling the error as needed
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while retrieving Wikipedia title for Wikidata ID: {WikidataId}", wikidataId);
                throw; // Consider rethrowing or handling the error as needed
            }
        }
    }
}
