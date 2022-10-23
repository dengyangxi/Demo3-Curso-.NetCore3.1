using Dapr;
using Micro.Common.Library;
using Micro.Common.Library.DaprPubSub;
using Micro.Common.Library.Entitys;
using Micro.Common.Library.Extensions;
using Micro.Order.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Micro.Order.API.Controller
{
    /// <summary>
    /// 接受消息
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SubscribeController : MicroBaseAPIController
    {
        private readonly ILogger<OrderController> _logger;

        /// <summary>
        /// DI 容器 依赖注入
        /// </summary>
        /// <param name="logger"></param>
        public SubscribeController(ILogger<OrderController> logger)
        {
            _logger = logger;
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
            // var model = content.ToModel<BaseMessage<NotificationMessage>>();

            //打印日志消息
            _logger.LogInformation($"==============================Micro.Order.API.SubscribeController   SubMessage 消息:==============================");
            //格式化输出Json
            _logger.LogError(content);
            _logger.LogInformation($"==============================Micro.Order.API.SubscribeController   SubMessage==============================");

            return Ok(content);
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

            //将二进制流转换成 字符串
            string content = Encoding.GetEncoding("gb2312").GetString(buffer);

            //将字符串转换成实体类
            var model = content.ToModel<BaseMessage<NotificationMessage>>();

            //打印日志消息
            _logger.LogInformation($"==============================Micro.Order.API.SubscribeController   SubMessage 消息:==============================");
            _logger.LogError(model.ToJsonFormat());
            _logger.LogInformation($"==============================Micro.Order.API.SubscribeController   SubMessage==============================");

            return Ok(content);
        }
    }
}