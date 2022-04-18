using Microsoft.AspNetCore.Mvc;

namespace Micro.Service.Hotel.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Json(new { Code = 200, Msg = "hello, 请求正常！~ " });
        }
    }
}
