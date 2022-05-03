using Micro.Order.API.Common;
using Microsoft.AspNetCore.Mvc;

namespace Micro.Order.API.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {

        private readonly ILogger<OrderController> _logger;

        /// <summary>
        /// IOC 容器 依赖注入
        /// </summary>
        /// <param name="logger"></param>
        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }



        [HttpGet(Name = "Index")]
        public IActionResult Index()
        {
            return Ok(new { Data = "结果集..ing", IsError = false, Msg = "请求成功, Order Index" });
        }


        [HttpGet]
        [Route("GetOrder")]
        public IActionResult GetOrder(string orderID = "")
        {

            orderID = orderID ?? "R00008880000888";

            return Ok(
                        new
                        {
                            IsError = false,
                            Data = new
                            {
                                GuestMobile = "13888888888",
                                GuestName = "菜徐坤",
                                OrderID = orderID
                            },
                            Msg = $"请求成功,用户需要获取订单{orderID}的信息"
                        }.ToJsonFormat()
                );

        }

    }
}
