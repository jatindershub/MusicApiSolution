using ArtistInfo.Api.Services.MusicBrainz;
using ArtistInfo.Api.Services.Wikipedia;
using ArtistInfo.Api.Services.Wikidata;
using ArtistInfo.Api.Services.CoverArtArchive;
using MusicApi.Models;
using MusicApi.Contracts.Artist;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace MusicApi.Services
{
    public class ArtistService : IArtistService
    {
        private readonly IMusicBrainzService _musicBrainzService;
        private readonly IWikidataService _wikidataService;
        private readonly IWikipediaService _wikipediaService;
        private readonly ICoverArtArchiveService _coverArtService;

        private static readonly SemaphoreSlim _semaphore = new (1, 1);
        private static readonly MemoryCache _cache = new (new MemoryCacheOptions());

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
            if (_cache.TryGetValue(mbid, out ArtistResponse cachedResponse))
            {
                return cachedResponse;
            }

            await _semaphore.WaitAsync();
            try
            {
                // Delay to respect the rate limit, but only if needed
                await Task.Delay(1000);

                var stopwatch = Stopwatch.StartNew();

                try
                {
                    var artistData = await _musicBrainzService.GetArtistAsync(mbid);
                    Console.WriteLine($"MusicBrainz API call took {stopwatch.ElapsedMilliseconds} ms");

                    var releaseGroups = (JArray)artistData["release-groups"] ?? new JArray();
                    var releaseGroupIds = releaseGroups.Select(rg => rg["id"]?.ToString()).ToList();
                    var titles = releaseGroups.Select(rg => rg["title"]?.ToString()).ToList();

                    var coverArtsTask = _coverArtService.FetchCoverArtsAsync(releaseGroupIds);
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

                    var coverArts = await coverArtsTask;
                    var modelAlbums = releaseGroupIds
                        .Zip(titles, (id, title) => new { id, title })
                        .Zip(coverArts, (group, coverArt) => new Album(group.title, group.id, coverArt))
                        .ToList();

                    var contractAlbums = modelAlbums.Select(MapToContractAlbum).ToList();

                    var artistResponse = new ArtistResponse(mbid, description, contractAlbums);
                    _cache.Set(mbid, artistResponse, TimeSpan.FromMinutes(5));

                    return artistResponse;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching artist data: {ex.Message}");
                    throw; // Consider returning a default ArtistResponse or handling the error more gracefully
                }
            }
            finally
            {
                _semaphore.Release();
            }
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
