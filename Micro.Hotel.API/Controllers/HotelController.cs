using Micro.Hotel.API.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Micro.Hotel.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HotelController : ControllerBase
    {

        private readonly ILogger<HotelController> _logger;

        /// <summary>
        /// IOC 依赖注入 ILogger
        /// </summary>
        /// <param name="logger"></param>
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




        [HttpGet]
        [Route("Log")]
        public IActionResult Log(string msg = "")
        {
           

            // #TODU 业务日志
            _logger.LogInformation($"请求日志Info：{msg}");
            _logger.LogError($"请求日志Error：{msg}");
            _logger.LogWarning($"请求日志Warning：{msg}");


            //#TODU 业务代码


            return Ok(new { Data = "Log日志..ing", IsError = false, Msg = "请求成功, Hotel Log" });
        }

    }
}
