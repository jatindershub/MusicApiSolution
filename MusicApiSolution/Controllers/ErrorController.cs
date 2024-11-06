using Microsoft.AspNetCore.Mvc;

namespace ArtistInfo.Api.Controllers
{
    public class ErrorController : ControllerBase
    {
        [Route("/error")]
        public ActionResult Error()
        {
            return Problem();
        }
    }
}
