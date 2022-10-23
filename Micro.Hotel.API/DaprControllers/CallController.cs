using Dapr.Client;

using Microsoft.AspNetCore.Mvc;

using Micro.Order.API.GrpcProtos;
using Micro.Common.Library.Entitys;
using Micro.Common.Library;


/*
 * Dapr :
    CNCF云原生基金会：  https://www.cncf.io/projects/dapr/ 
    Dapr源代码Github:   https://github.com/dapr/dapr        
    Dapr官方中文文档：  https://docs.dapr.io/zh-hans/

    适用于 Go         的 Dapr SDK: https://github.com/dapr/go-sdk
    适用于 Java       的 Dapr SDK: https://github.com/dapr/java-sdk
    适用于 JavaScript 的 Dapr SDK: https://github.com/dapr/js-sdk
    适用于 Python     的 Dapr SDK: https://github.com/dapr/python-sdk
    适用于 .NET Core  的 Dapr SDK: https://github.com/dapr/dotnet-sdk
    适用于 Rust       的 Dapr SDK: https://github.com/dapr/rust-sdk
    适用于 C++        的 Dapr SDK: https://github.com/dapr/cpp-sdk
    适用于 PHP        的 Dapr SDK: https://github.com/dapr/php-sdk

*/

/*
 * 1. 服务调用 (Service Invocation):
        跨服务调用允许进行远程方法调用(包括重试)，不管处于任何位置，只需该服务托管于受支持的环境即可。 
 */
namespace Micro.Hotel.API.Controllers
{
    /// <summary> 
    /// 1. Service Invocation       服务调用
    /// 
    ///   Dapr : https://dapr.io/  
    ///     1. Service Invocation       服务调用         https://docs.dapr.io/developing-applications/building-blocks/service-invocation/service-invocation-overview/
    ///     2. Publish & Subscribe      发布和订阅       https://docs.dapr.io/developing-applications/building-blocks/pubsub/pubsub-overview/
    ///     3. Secret Management        秘密管理         https://docs.dapr.io/developing-applications/building-blocks/secrets/secrets-overview/
    ///     4. Input/Output Bindings    输入/输出绑定    https://docs.dapr.io/developing-applications/building-blocks/bindings/bindings-overview/
    ///     5. State Management         状态管理         https://docs.dapr.io/developing-applications/building-blocks/state-management/state-management-overview/
    ///     6. Virtual Actors           Actor模型        https://docs.dapr.io/developing-applications/building-blocks/actors/actors-overview/
    ///     
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CallController : MicroBaseAPIController
    {

        #region 0. DI 注入对象
        /// <summary>
        /// 日志对象
        /// </summary>
        private readonly ILogger<CallController> _logger;

        /// <summary>
        ///   Dapr 边车客户端 
        ///       注： DaprClient 并非代码入侵试， 它只是将一些通用的http 的各种 请求封装成一个方法。
        ///            您也可以使用： DaprClient   发起 http / Grpc 请求
        /// </summary>
        private readonly DaprClient _daprClient;

        #endregion 


        #region 0. 构造函数
        /// <summary>
        /// 构造函数
        ///       DI—Dependency Injection  依赖注入
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="daprClient"></param>
        public CallController(ILogger<CallController> logger, DaprClient daprClient)
        {
            //日志对象
            _logger = logger;

            //Dapr 边车
            _daprClient = daprClient;
        }
        #endregion


        #region 一、 服务调用  使用  HTTP  通讯


        /// <summary>
        /// 1.1)   发起一个请求:  HTTP 通讯 的服务调用...
        ///     手动实例化
        ///     Service Call Service
        ///     Hotel  ---> Call----> Order
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetOrderInfoV2Http")]
        [HttpPost("GetOrderInfoV2Http")]
        public async Task<ActionResult> GetOrderInfoV2HttpAsync()
        {
            try
            {
                //实例化 DaprClient 请求对象
                using var _daprClient = DaprClient.CreateInvokeHttpClient();
                //发起请求
                var result = await _daprClient.GetAsync($"http://{AppIdList.MicroOrderAPI}/Order/Index");
                //服务返回值
                var readResult = await result.Content.ReadAsStringAsync();
                //返回结果对象 
                return Success(readResult);
            }
            catch (Exception ex)
            {
                return Failed<string>(ex.Message);
            }
        }


        /// <summary>
        /// 1.2)   发起一个请求:  HTTP 通讯 的服务调用...
        ///     DI 依赖注入
        ///     Service Call Service
        ///     Hotel  ---> Call----> Order
        /// </summary>
        /// <returns></returns> 
        [HttpPost("GetOrderInfoV3Http")]
        public async Task<ActionResult> GetOrderInfoV3HttpAsync(OrderInfoRequest request)
        {
            try
            {

                //发起请求
                var result = await _daprClient.InvokeMethodAsync<OrderInfoRequest, BaseModel<OrderInfoModel>>
                    (
                        HttpMethod.Post,    //请求类型： Get Post Delete  Put Patch ....等 
                        AppIdList.MicroOrderAPI,       // 需要调用的微服务名称. 只需指定微服务名字
                        "/Order/GetOrder",   // 需要调用的方法
                        request           //  请求参数 
                    );

                return Ok(result);

            }
            catch (Exception ex)
            {
                return Failed<string>(ex.Message);
            }
        }

        #endregion


        #region 二、 服务调用  使用  Grpc  通讯


        /// <summary>
        /// 2.1)   发起一个请求:  Grpc 通讯 的服务调用...
        ///     手动实例化
        ///     Service Call Service
        ///     Hotel  ---> Call----> Order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns> 
        [HttpPost("GetOrderInfoV1ByGrpc")]
        public async Task<ActionResult> GetOrderInfoV1ByGrpcAsync(OrderInfoRequest request)
        {
            // 实例化对象
            using var daprClient = new DaprClientBuilder().Build();
            try
            {


                // Hotel 微服务----》通过 SidCar 以 Grpc的通讯方式--》调用另外一个微服务 Order 的 SidCar
                //      注意：发起Grpc 请求方法 InvokeMethodGrpcAsync ,而不是 Http 请求方法 InvokeMethodAsync
                var result = await daprClient.InvokeMethodGrpcAsync<GetOrderRequest, GetOrderResponse>
                    (
                        //  AppID   （微服务名称）
                        AppIdList.MicroOrderAPI,
                        //  Method   (方法名称)
                        GrpcMethodEnum.OrderAPI.GetOrderByGrpc.ToString(),
                        // 请求参数
                        new GetOrderRequest { HotelCd = request.HotelCd, OrderID = request.OrderID }
                    );

                // 返回结果给Json数据前端：
                // Micro Hotel---->Micro Order 
                return Success(result);
            }
            catch (Exception ex)
            {

                return Failed<string>(ex.Message);
            }
        }



        /// <summary>
        /// 2.2)   发起一个请求:  Grpc 通讯 的服务调用...
        ///     DI 依赖注入
        ///     Service Call Service
        ///     Hotel  ---> Call----> Order
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns> 
        [HttpPost("GetOrderInfoV2ByGrpc")]
        public async Task<ActionResult> GetOrderInfoV2ByGrpcAsync(OrderInfoRequest request)
        {

            // Hotel 微服务----》通过 SidCar 以 Grpc的通讯方式--》调用另外一个微服务 Order 的 SidCar
            //      注意：发起Grpc 请求方法 InvokeMethodGrpcAsync ,而不是 Http 请求方法 InvokeMethodAsync
            var result = await _daprClient.InvokeMethodGrpcAsync<GetOrderRequest, GetOrderResponse>
                (
                    //  AppID   （微服务名称）
                    AppIdList.MicroOrderAPI,
                    //  Method   (方法名称)
                    GrpcMethodEnum.OrderAPI.AddRoomGuest.ToString(),
                    // 请求参数
                    new GetOrderRequest { HotelCd = request.HotelCd, OrderID = request.OrderID }
                );

            //返回结果给Json数据前端：
            // Micro Hotel---->Micro Order 
            return Ok(result);
        }

        #endregion


    }
}
