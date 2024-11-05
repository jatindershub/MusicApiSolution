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
            var response = _artistService.GetArtistAsync(mbid);
            return Ok(response);
        }

        
    }
}
