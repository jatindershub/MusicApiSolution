using Newtonsoft.Json.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace ArtistInfo.Api.Services.Wikipedia
{
    public class WikipediaService : IWikipediaService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WikipediaService> _logger;

        public WikipediaService(HttpClient httpClient, ILogger<WikipediaService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<string> GetDescriptionAsync(string wikipediaTitle)
        {
            if (string.IsNullOrEmpty(wikipediaTitle))
            {
                _logger.LogWarning("Wikipedia title is null or empty.");
                return null;
            }

            try
            {
                var encodedTitle = System.Web.HttpUtility.UrlEncode(wikipediaTitle);
                var wikipediaApiUrl = $"https://en.wikipedia.org/w/api.php?action=query&titles={encodedTitle}&prop=extracts&exintro&explaintext&format=json";
                var response = await _httpClient.GetAsync(wikipediaApiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Error fetching Wikipedia data: StatusCode={response.StatusCode}, Reason={response.ReasonPhrase}");
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var wikipediaJson = JObject.Parse(content);
                var page = wikipediaJson["query"]?["pages"]?.First?.First;

                if (page == null)
                {
                    _logger.LogWarning("No valid page found in the Wikipedia response.");
                    return null;
                }

                return page["extract"]?.ToString() ?? string.Empty;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HttpRequestException occurred while fetching Wikipedia data: {ex.Message}");
                return null;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"JsonException occurred while parsing Wikipedia response: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error occurred: {ex.Message}");
                return null;
            }
        }

    }
}
