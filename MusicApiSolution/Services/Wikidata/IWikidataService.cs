namespace ArtistInfo.Api.Services.Wikidata
{
    public interface IWikidataService
    {
        Task<string> GetWikipediaTitleAsync(string wikidataId);
    }
}
