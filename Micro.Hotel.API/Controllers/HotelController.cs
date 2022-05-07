using Micro.Hotel.API.Common;
using Microsoft.AspNetCore.Mvc;

namespace Micro.Hotel.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HotelController : ControllerBase
    {

        private readonly ILogger<HotelController> _logger;

        public HotelController(ILogger<HotelController> logger)
        {
            _logger = logger;
        }




        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            return Ok(new { Data = "结果集..ing", IsError = false, Msg = "请求成功, Hotel Index" });
        }


        [HttpGet]
        [Route("GetHotel")]
        public IActionResult GetHotel(string hotelCd = "")
        {

            hotelCd = hotelCd ?? "021098";

            return Ok(
                        new
                        {
                            IsError = false,
                            Data = new
                            {
                                HotelName = "柳州路店",
                                Address = "上海市xx路xx号",
                                HotelCd = hotelCd
                            },
                            Msg = $"请求成功,用户需要获取酒店{hotelCd}的信息"
                        }.ToJsonFormat()
                );

        }
         
         
    }
}
