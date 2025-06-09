// Controllers/RootController.cs
using Microsoft.AspNetCore.Mvc;

namespace GameService.Controllers
{
    [ApiController]
    [Route("/")]
    public class RootController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                status = "GameService UP",
                timestamp = DateTime.UtcNow
            });
        }
    }
}
