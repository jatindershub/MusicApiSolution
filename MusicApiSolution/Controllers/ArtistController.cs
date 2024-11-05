using Microsoft.AspNetCore.Mvc;
using MusicApi.Contracts.Artist;
using MusicApi.Models;
using MusicApi.Services;

namespace MusicApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ArtistController : ControllerBase
    {
        // todo: dependency injection
        private readonly IArtistService _artistService;
        public ArtistController(IArtistService artistService)
        {
            _artistService = artistService;
        }

        [HttpGet("/{mbid}")]
        public IActionResult GetArtist(string mbid)
        {
            var artist = _artistService.GetArtist(mbid);

            // mapper Albums fra models til contracts
            var contractAlbums = artist.Albums.Select(MapToContractAlbum).ToList();

            var response = new ArtistResponse(artist.Mbid, artist.Description, contractAlbums);

            return Ok(response);
        }

        // todo: find ud af om den her helper metode skal være her, eller om den skal et andet sted hen - måske over i en helper manager?
        private static Contracts.Artist.Album MapToContractAlbum(Models.Album modelAlbum)
        {
            return new Contracts.Artist.Album(
                modelAlbum.Title,
                modelAlbum.Id,
                modelAlbum.Image
                );
        }

        
    }
}
