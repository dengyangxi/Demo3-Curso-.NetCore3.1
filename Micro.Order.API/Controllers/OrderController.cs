
using Microsoft.AspNetCore.Mvc;
using Micro.Common.Library;
using Micro.Common.Library.Entitys;

namespace Micro.Order.API.Controllers
{

    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class OrderController : MicroBaseAPIController
    {

        private readonly ILogger<OrderController> _logger;

        /// <summary>
        /// DI 容器 依赖注入
        /// </summary>
        /// <param name="logger"></param>
        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }



        [HttpGet(Name = "Index")]
        [Route("Index")]
        public IActionResult Index()
        {
            return Success("来自 Micro.Order.API 结果集..ing", "请求成功, Micro.Order.API 微服务的 Order/Index 方法");

        }


        /// <summary>
        ///   获取订单详细信息
        /// </summary>
        /// <param name="hotelcd">酒店编号</param>
        /// <param name="orderID">订单编号</param>
        /// <returns></returns>
        [HttpPost("GetOrder")]

        public IActionResult GetOrder(OrderInfoRequest request)
        {

            return Success<OrderInfoModel>(new OrderInfoModel()
            {
                GuestMobile = "13888888888",
                GuestName = "菜徐坤",
                OrderID = request.OrderID,
                RoomInfo = new List<RoomInfoModel>()
                {
                     new RoomInfoModel (){
                             RmNo ="8888",
                             RmType="普通标间"
                     },
                       new RoomInfoModel (){
                             RmNo ="999",
                             RmType="普通双人房"
                     },
                }
            }, $"请求成功,用户需要获取订单{request.OrderID}的信息");

            //.ToJsonFormat()

        }

    }
}
