using Microsoft.AspNetCore.Mvc;

namespace news_ai_project.server.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { message = "API is working!" });
        }
    }
}
