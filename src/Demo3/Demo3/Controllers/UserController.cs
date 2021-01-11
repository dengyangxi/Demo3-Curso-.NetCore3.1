using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace Demo3.Controllers
{
    /// <summary>
    /// Controller of CRUD of User
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IActionResult GetList()
        {
            _logger.LogInformation("test log information");
            return Content("OK");
        }

    }
}
