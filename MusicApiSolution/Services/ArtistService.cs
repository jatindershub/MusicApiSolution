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
        private readonly ILogger<ArtistService> _logger;

        private static readonly SemaphoreSlim _semaphore = new(1, 1);
        private static readonly MemoryCache _cache = new(new MemoryCacheOptions());

        public ArtistService(IMusicBrainzService musicBrainzService, IWikidataService wikidataService,
            IWikipediaService wikipediaService, ICoverArtArchiveService coverArtService, ILogger<ArtistService> logger)
        {
            _musicBrainzService = musicBrainzService;
            _wikidataService = wikidataService;
            _wikipediaService = wikipediaService;
            _coverArtService = coverArtService;
            _logger = logger;
        }

        public void test()
        {
            Console.WriteLine("lolleren");
        }

        public async Task<ArtistResponse> GetArtistAsync(string mbid)
        {
            if (_cache.TryGetValue(mbid, out ArtistResponse cachedResponse))
            {
                _logger.LogInformation($"Cache hit for MBID: {mbid}");
                return cachedResponse;
            }

            await _semaphore.WaitAsync();
            try
            {
                // Delay to respect the rate limit, but only if needed. MusicBrainz: 1 request/sec (anonymous) or 5 requests/sec (authenticated).
                await Task.Delay(1000);

                var stopwatch = Stopwatch.StartNew();

                try
                {
                    _logger.LogInformation($"Fetching artist data for MBID: {mbid}");
                    var artistData = await _musicBrainzService.GetArtistAsync(mbid);

                    if (artistData == null)
                    {
                        _logger.LogWarning($"MusicBrainz API returned null for MBID: {mbid}");
                        return null;
                    }

                    _logger.LogInformation($"MusicBrainz API call took {stopwatch.ElapsedMilliseconds} ms");

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

                    _logger.LogInformation($"Successfully fetched artist data for MBID: {mbid}");
                    return artistResponse;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error fetching artist data for MBID: {mbid}");
                    throw;
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
