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
        public async Task<IActionResult> GetArtist(string mbid)
        {
            var artistResponse = await _artistService.GetArtistAsync(mbid);
            if (artistResponse == null)
            {
                return NotFound(new { message = $"Artist with MBID '{mbid}' not found." });
            }
            return Ok(artistResponse);
        }
    }
}
