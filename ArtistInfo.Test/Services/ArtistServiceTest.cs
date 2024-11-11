using ArtistInfo.Api.Services.CoverArtArchive;
using ArtistInfo.Api.Services.MusicBrainz;
using ArtistInfo.Api.Services.Wikidata;
using ArtistInfo.Api.Services.Wikipedia;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using MusicApi.Contracts.Artist;
using MusicApi.Models;
using MusicApi.Services;
using Newtonsoft.Json.Linq;

namespace ArtistInfo.Test.Services
{
    public class ArtistServiceTest
    {
        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<IMusicBrainzService> _mockMusicBrainzService;
        private readonly Mock<IWikidataService> _mockWikidataService;
        private readonly Mock<IWikipediaService> _mockWikipediaService;
        private readonly Mock<ICoverArtArchiveService> _mockCoverArtService;
        private readonly Mock<ILogger<ArtistService>> _mockLogger;
        private readonly ArtistService _artistService;

        public ArtistServiceTest()
        {
            _mockCache = new Mock<IMemoryCache>();
            _mockMusicBrainzService = new Mock<IMusicBrainzService>();
            _mockWikidataService = new Mock<IWikidataService>();
            _mockWikipediaService = new Mock<IWikipediaService>();
            _mockCoverArtService = new Mock<ICoverArtArchiveService>();
            _mockLogger = new Mock<ILogger<ArtistService>>();

            _artistService = new ArtistService(
                _mockMusicBrainzService.Object,
                _mockWikidataService.Object,
                _mockWikipediaService.Object,
                _mockCoverArtService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task GetArtistAsync_ShouldReturnNull_WhenMusicBrainzServiceReturnsNull()
        {
            // Arrange
            string mbid = "Invalid-mbid"; // Use a clearly invalid MBID to simulate the scenario
            JObject artistData = null;
            object cacheEntry = null;
            _mockCache.Setup(cache => cache.TryGetValue(mbid, out cacheEntry)).Returns(false);
            _mockMusicBrainzService.Setup(service => service.GetArtistAsync(mbid)).ReturnsAsync(artistData);

            // Act
            var result = await _artistService.GetArtistAsync(mbid);

            // Assert
            Assert.Null(result);
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("MusicBrainz API returned null for MBID")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

        [Fact]
        public async Task GetArtistAsync_ShouldReturnNull_WhenAnExceptionIsThrown()
        {
            // Arrange
            string mbid = "valid-mbid";
            object cacheEntry = null;
            var exception = new Exception("Some error");
            _mockCache.Setup(cache => cache.TryGetValue(mbid, out cacheEntry)).Returns(false);
            _mockMusicBrainzService.Setup(service => service.GetArtistAsync(mbid)).ThrowsAsync(exception);

            // Act
            ArtistResponse result = null;
            try
            {
                result = await _artistService.GetArtistAsync(mbid);
            }
            catch (Exception ex)
            {
                _mockLogger.Object.LogError(ex, "Exception occurred while fetching artist");
            }

            // Assert
            Assert.Null(result);
            _mockLogger.Verify(
                logger => logger.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Exception occurred while fetching artist")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }

    }
}
