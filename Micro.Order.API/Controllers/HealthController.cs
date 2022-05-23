using Microsoft.AspNetCore.Mvc;

namespace Micro.Order.API.Controllers
{

    /// <summary>
    /// 提供 k8s pod 监控检查
    ///     请勿随意调整
    /// </summary>
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

        public IActionResult Get()
        {
            return Ok();
        }
    }
}
