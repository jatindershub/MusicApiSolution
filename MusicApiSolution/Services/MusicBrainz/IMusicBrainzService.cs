using Newtonsoft.Json.Linq;

namespace ArtistInfo.Api.Services.MusicBrainz
{
    public interface IMusicBrainzService
    {
        Task<JObject> GetArtistAsync(string mbid);
    }
}
