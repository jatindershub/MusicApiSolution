using ArtistInfo.Api.Services.MusicBrainz;
using ArtistInfo.Api.Services.Wikipedia;
using ArtistInfo.Api.Services.Wikidata;
using ArtistInfo.Api.Services.CoverArtArchive; // todo: find ud af om man bare kan kalde /Services i stedet for individuelle mapper
using MusicApi.Models;


namespace MusicApi.Services
{
    public class ArtistService : IArtistService
    {
        //private static readonly Dictionary<Guid, Artist> _artist = new(); // todo: denne skal bruges til når man skal implementere create, update, delete

        private readonly IMusicBrainzService _musicBrainzService;
        private readonly IWikidataService _wikidataService;
        private readonly IWikipediaService _wikipediaService;
        private readonly ICoverArtArchiveService _coverArtService;

        // metode med dummy data
        /*public Artist GetArtist(string mbid)
        {
            var album1 = new Album() // todo skal slettes
            {
                Title = "Nevermind", 
                Id = "1b022e01-4da6-387b-8658-8678046e4cef", 
                Image = "https://coverartarchive.org/release/a146429a-cedc-3ab0-9e41-1aaf5f6cdc2"
            };

            var album2 = new Album()
            {
                Title = "In Utero",
                Id = "4b02a382-86f0-4568-a9e0-0c567cf8f5b6",
                Image = "https://coverartarchive.org/release/0f8a7e91-308d-470a-b5f8-1d93b6a26b08"
            };

            var albums = new List<Album>
            {
                album1,
                album2,
            };

            var artist = new Artist("5b11f4ce-a62d-471e-81fc-a69a8278c7da", "Nirvana was an American rock band formed in Aberdeen, Washington, in 1987.", albums);

            return artist;
        }*/
        public Artist GetArtist(string mbid)
        {
            // herinde skal logikken være til når vi skal kalde de forskellige api'er fra /services

            //var artist = new Artist("","",Liste af albums)


        }
    }
}
