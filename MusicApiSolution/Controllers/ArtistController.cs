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

            var response = new ArtistResponse(artist.Mbid, artist.Description, artist.Albums);

            return Ok(response);
        }

        
    }
}
