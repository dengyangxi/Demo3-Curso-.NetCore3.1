using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Micro.Common.Library;
using Dapr;
using Micro.Common.Library.Entitys;
using Micro.Common.Library.Extensions;
using Micro.Common.Library.DaprPubSub;

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
 * 2. 发布和订阅 (Publish & Subscribe):
        发布事件和订阅主题。
 */
namespace Micro.Hotel.API.DaprControllers
{

    /// <summary> 
    /// 2. Publish & Subscribe      发布和订阅
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
    public class PubSubController : MicroBaseAPIController
    {
        #region 0. DI 注入对象
        /// <summary>
        /// 日志对象
        /// </summary>
        private readonly ILogger<PubSubController> _logger;

        /// <summary>
        ///   Dapr 边车客户端 
        ///       注： DaprClient 并非代码入侵试， 它只是将一些通用的http 的各种 请求封装成一个方法。
        ///            您也可以使用： DaprClient   发起 http / Grpc 请求
        /// </summary>
        private readonly DaprClient _daprClient;

        #endregion 

        public PubSubController(ILogger<PubSubController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;

        }


        #region 一、 通过代码 指定接收消息. 




        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost("PubMessageByCode")]
        public async Task<ActionResult> PubMessageByCodeAsync(NotificationMessage message)
        {
            //发送消息
            await _daprClient.PublishEventAsync<NotificationMessage>(
                PubSubComponent.PubSubRedisComponentName,        // 消息pub sub 的名称, 即：  1.redis-pubsub.yaml 文件 metadata : name
                PubSubTopic.MyCodeRedisTopicName,                // topic
                message);                                        // 消息具体内容
            return Success("OK");
        }


        /// <summary>
        /// 指定接受消息
        ///         [Topic("pubsub", "inventory", "event.type ==\"widget\"", 1)]
        /// </summary>
        /// <returns></returns>
        [Topic(PubSubComponent.PubSubRedisComponentName, PubSubTopic.MyCodeRedisTopicName)]
        [HttpPost("SubMessageByCode")]
        public async Task<ActionResult> SubMessageByCodeAsync()
        {
            Stream stream = Request.Body;
#pragma warning disable CS8629 // 可为 null 的值类型可为 null。
            byte[] buffer = new byte[Request.ContentLength.Value];
#pragma warning restore CS8629 // 可为 null 的值类型可为 null。
            stream.Position = 0L;
            //读取消息
            await stream.ReadAsync(buffer, 0, buffer.Length);

            //将二进制流转换成 字符串。
            string content = Encoding.GetEncoding("gb2312").GetString(buffer);


            //将字符串转换成实体类
            //  var model = content.ToModel<BaseMessage<NotificationMessage>>();


            //打印日志消息
            _logger.LogInformation($"==============================Micro.Hotel.API.DaprControllers   SubMessage 消息:==============================");
            //格式化输出Json
            _logger.LogError(content);
            _logger.LogInformation($"==============================Micro.Hotel.API.DaprControllers   SubMessage==============================");

            return Ok(content);


        }

        #endregion


        #region 一、 通过yaml 文件指定接收消息. 


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost("PubMessage")]
        public async Task<ActionResult> PubMessageAsync(NotificationMessage message)
        {
            _ = message.HotelCd ?? "021040";

            //发布消息
            await _daprClient.PublishEventAsync<NotificationMessage>
                (
                   "redis-pubsub-component",    // 消息pub sub 的名称, 即：  1.redis-pubsub.yaml 文件 metadata : name
                   "my_yaml_topic",             // topic
                   message                      // 消息具体内容
                );
            return Success("OK");
        }


        /// <summary>
        /// 接收消息 （被动接受消息） 
        /// </summary> 
        /// <returns></returns>
        [HttpPost("SubMessage")]
        public async Task<ActionResult> SubMessageAsync()
        {
            Stream stream = Request.Body;
#pragma warning disable CS8629 // 可为 null 的值类型可为 null。
            byte[] buffer = new byte[Request.ContentLength.Value];
#pragma warning restore CS8629 // 可为 null 的值类型可为 null。
            stream.Position = 0L;
            //读取消息
            await stream.ReadAsync(buffer, 0, buffer.Length);

            //将二进制流转换成 字符串。
            string content = Encoding.GetEncoding("gb2312").GetString(buffer);

            //将字符串转换成实体类
            //   var model = content.ToModel<BaseMessage<NotificationMessage>>();


            //打印日志消息
            _logger.LogInformation($"==============================Micro.Hotel.API.DaprControllers   SubMessage 消息:==============================");
            _logger.LogError(content);
            _logger.LogInformation($"==============================Micro.Hotel.API.DaprControllers   SubMessage==============================");

            return Ok(content);
        }


        #endregion

    }
}
