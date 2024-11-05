namespace ArtistInfo.Api.Services.Wikipedia
{
    public interface IWikipediaService
    {
        Task<string> GetDescriptionAsync(string wikipediaTitle);
    }
}
