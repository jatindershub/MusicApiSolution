using Microsoft.AspNetCore.Mvc;

namespace ArtistInfo.Api.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        [HttpGet] // Explicitly defining the HTTP method
        public ActionResult Error()
        {
            return Problem();
        }
    }
}