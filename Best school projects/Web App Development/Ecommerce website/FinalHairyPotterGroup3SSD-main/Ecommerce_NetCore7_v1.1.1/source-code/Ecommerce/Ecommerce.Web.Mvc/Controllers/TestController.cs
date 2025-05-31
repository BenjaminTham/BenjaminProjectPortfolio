using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Web.Mvc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
