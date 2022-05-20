using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using System.Text;
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
 * 4. 输入/输出绑定 (Input/Output Bindings):
        Dapr的Bindings是建立在事件驱动架构的基础之上的。通过建立触发器与资源的绑定，
        可以从任何外部源（例如数据库，队列，文件系统等）接收和发送事件，而无需借助消息队列，即可实现灵活的业务场景。
 */
namespace Micro.Hotel.API.DaprControllers
{
    /// <summary> 
    /// 4. Input/Output Bindings    输入/输出绑定
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
    public class BindingController : MicroBaseAPIController
    {
        #region 0. DI 注入对象
        /// <summary>
        /// 日志对象
        /// </summary>
        private readonly ILogger<BindingController> _logger;

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
        public BindingController(ILogger<BindingController> logger, DaprClient daprClient)
        {
            //日志对象
            _logger = logger;

            //Dapr 边车
            _daprClient = daprClient;
        }
        #endregion


        /// <summary>
        /// 输入绑定信息 
        /// </summary>
        /// <returns></returns>
        [HttpPost("InputBindingMessage")]
        public ActionResult PrintBindingMessage()
        {
            Stream stream = Request.Body;
#pragma warning disable CS8629 // 可为 null 的值类型可为 null。
            byte[] buffer = new byte[Request.ContentLength.Value];
#pragma warning restore CS8629 // 可为 null 的值类型可为 null。
            stream.Position = 0L;
            stream.ReadAsync(buffer, 0, buffer.Length);
            string content = Encoding.UTF8.GetString(buffer);
            _logger.LogInformation(".............binding............." + content);
            return Ok();
        }


        /// <summary>
        /// 输出绑定
        /// </summary>
        /// <param name="daprClient"></param>
        /// <returns></returns>
        [HttpGet("OutputBindingMessage")]
        public async Task<ActionResult> OutputAsync(string message)
        {
            await _daprClient.InvokeBindingAsync(
                "api/Binding/OutputBindingMessage",
                "create",
                message  // create  get   delete  list
                );
            return Success("OK");
        }


        /// <summary>
        ///  定时任务调度
        /// </summary>
        /// <returns></returns>
        [HttpPost("CornBindingMessage")]
        public ActionResult Corn()
        {
            _logger.LogInformation(".............Corn Binding Message.............");

            return Ok();
        }
    }
}
