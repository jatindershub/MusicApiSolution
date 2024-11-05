using ArtistInfo.Api.Services.MusicBrainz;
using ArtistInfo.Api.Services.Wikipedia;
using ArtistInfo.Api.Services.Wikidata;
using ArtistInfo.Api.Services.CoverArtArchive; // todo: find ud af om man bare kan kalde /Services i stedet for individuelle mapper
using MusicApi.Models;
using MusicApi.Contracts.Artist;
using Newtonsoft.Json.Linq;


namespace MusicApi.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IMusicBrainzService _musicBrainzService;
        private readonly IWikidataService _wikidataService;
        private readonly IWikipediaService _wikipediaService;
        private readonly ICoverArtArchiveService _coverArtService;

        public ArtistService(IMusicBrainzService musicBrainzService, IWikidataService wikidataService,
            IWikipediaService wikipediaService, ICoverArtArchiveService coverArtService)
        {
            _musicBrainzService = musicBrainzService;
            _wikidataService = wikidataService;
            _wikipediaService = wikipediaService;
            _coverArtService = coverArtService;
        }

        public async Task<ArtistResponse> GetArtistAsync(string mbid)
        {
            var artistData = await _musicBrainzService.GetArtistAsync(mbid);
            var releaseGroups = (JArray)artistData["release-groups"];
            var modelAlbums = new List<Album>();

            // Create Model Albums
            foreach (var releaseGroup in releaseGroups)
            {
                var id = releaseGroup["id"]?.ToString();
                var title = releaseGroup["title"]?.ToString();
                var imageUrl = await _coverArtService.GetCoverArtAsync(id);

                modelAlbums.Add(new Album(title, id, imageUrl));
            }

            // Map to Contract Albums
            var contractAlbums = modelAlbums.Select(MapToContractAlbum).ToList();

            var relations = (JArray)artistData["relations"];
            var wikidataRelation = relations?.FirstOrDefault(r => r["type"]?.ToString() == "wikidata");
            string description = null;

            if (wikidataRelation != null)
            {
                var wikidataUrl = wikidataRelation["url"]?["resource"]?.ToString();
                var wikidataId = wikidataUrl?.Split('/').Last();
                var wikipediaTitle = await _wikidataService.GetWikipediaTitleAsync(wikidataId);

                description = await _wikipediaService.GetDescriptionAsync(wikipediaTitle);
            }

            return new ArtistResponse(mbid, description, contractAlbums);
        }


        private static AlbumDto MapToContractAlbum(Album modelAlbum)
        {
            return new AlbumDto(
                modelAlbum.Title,
                modelAlbum.Id,
                modelAlbum.Image
            );
        }
    }
}
