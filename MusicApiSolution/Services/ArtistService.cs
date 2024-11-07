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

        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private static readonly MemoryCache _cache = new MemoryCache(new MemoryCacheOptions());

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
            // Ensure that only one request is being sent per second
            await _semaphore.WaitAsync();

            try
            {
                if (_cache.TryGetValue(mbid, out ArtistResponse cachedResponse))
                {
                    return cachedResponse;
                }

                // Delay to respect the 1 request per second rate limit
                await Task.Delay(1000);

                var stopwatch = Stopwatch.StartNew();

                // Logging the time taken for each API call
                stopwatch.Restart();
                var artistData = await _musicBrainzService.GetArtistAsync(mbid);
                stopwatch.Stop();
                Console.WriteLine($"MusicBrainz API call took {stopwatch.ElapsedMilliseconds} ms");

                var releaseGroups = (JArray)artistData["release-groups"];
                var releaseGroupIds = releaseGroups.Select(rg => rg["id"]?.ToString()).ToList();
                var titles = releaseGroups.Select(rg => rg["title"]?.ToString()).ToList();

                stopwatch.Restart();
                var coverArts = await _coverArtService.FetchCoverArtsAsync(releaseGroupIds);
                stopwatch.Stop();
                Console.WriteLine($"CoverArtArchive API call took {stopwatch.ElapsedMilliseconds} ms");

                // Ensure all lists are of equal size using Zip
                var modelAlbums = releaseGroupIds
                    .Zip(titles, (id, title) => new { id, title })
                    .Zip(coverArts, (group, coverArt) => new Album(group.title, group.id, coverArt))
                    .ToList();

                // Map to Contract Albums
                var contractAlbums = modelAlbums.Select(MapToContractAlbum).ToList();

                var relations = (JArray)artistData["relations"];
                var wikidataRelation = relations?.FirstOrDefault(r => r["type"]?.ToString() == "wikidata");
                string description = null;

                if (wikidataRelation != null)
                {
                    var wikidataUrl = wikidataRelation["url"]?["resource"]?.ToString();
                    var wikidataId = wikidataUrl?.Split('/').Last();

                    stopwatch.Restart();
                    var wikipediaTitle = await _wikidataService.GetWikipediaTitleAsync(wikidataId);
                    stopwatch.Stop();
                    Console.WriteLine($"Wikidata API call took {stopwatch.ElapsedMilliseconds} ms");

                    stopwatch.Restart();
                    description = await _wikipediaService.GetDescriptionAsync(wikipediaTitle);
                    stopwatch.Stop();
                    Console.WriteLine($"Wikipedia API call took {stopwatch.ElapsedMilliseconds} ms");
                }

                var artistResponse = new ArtistResponse(mbid, description, contractAlbums);

                // Cache the response for future requests
                _cache.Set(mbid, artistResponse, TimeSpan.FromMinutes(5));

                return artistResponse;
            }
            finally
            {
                // Release the semaphore to allow the next request
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
