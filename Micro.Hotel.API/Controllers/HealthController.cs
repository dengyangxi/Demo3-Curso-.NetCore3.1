using Microsoft.AspNetCore.Mvc;

namespace Micro.Hotel.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    { 
        /// <summary>
        /// 心跳健康检查
        /// </summary>
        /// <returns></returns>
        [HttpGet("Index")]
        public IActionResult Index()
        {
            return Ok();
        }
        [HttpGet("Get")]
        public IActionResult Get()
        {
            return Ok();
        }
    }
}
