using Dapr.Actors;
using Dapr.Actors.Client;
using Micro.Common.Library.InterfaceActor;
using Microsoft.AspNetCore.Mvc;
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
 * 6. Actor模型 (Virtual Actors):
  Actor模型 = 状态 + 行为 + 消息。一个应用/服务由多个Actor组成，每个Actor都是一个独立的运行单元，拥有隔离的运行空间，
  在隔离的空间内，其有独立的状态和行为，不被外界干预，Actor之间通过消息进行交互，而同一时刻，每个Actor只能被单个线程执行，
  这样既有效避免了数据共享和并发问题，又确保了应用的伸缩性。 
  Dapr 在Actor模式中提供了很多功能，包括并发，状态管理，用于 actor 激活/停用的生命周期管理，以及唤醒 actor 的计时器和提醒器。

 */
namespace Micro.Hotel.API.Controllers
{

    /// <summary> 
    /// 6. Virtual Actors			Actor模型
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
    public class ActorsController : MicroBaseAPIController
    {


        /*
            orderapi||OrderRefundWorkflowActor||actor-id-1||actor-id-1
            <app-id>||<actor-type>||<actor-id>||<actor-id>
         */

        /// <summary>
        /// 提交一个审批流程-actor模型 并发控制
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpGet("{orderId}")]
        public async Task<ActionResult> OrderRefundApproveAsync(string orderId)
        {
            try
            {
                //初始化 Actor ID
                var actorId = new ActorId($"actor-id-{orderId}");   //order-refund-
                //创建一个订单退款的 Actor 模型
                var proxy = ActorProxy.Create<IWorkFlowActor>
                                                    (
                                                        actorId,            //ActorId
                                                        "WorkflowActor"     //Actor 实现的类型。  （一般填写类名称。  xxx.cs）
                                                    );

                var result = await proxy.OrderRefundApprove();
                //返回结果
                return Result(result);

            }
            catch (Exception ex)
            {
                return Failed<string>(ex.Message, 500);
            }
        }

        #region Reminder  

        [HttpGet("Reminder/{orderId}")]
        public async Task<ActionResult> ReminderAsync(string orderId)
        {
            //初始化 Actor ID
            var actorId = new ActorId("reminder-actor-id-" + orderId);
            //创建一个Reminder的 Actor 模型
            var proxy = ActorProxy.Create<IWorkFlowActor>(actorId, "WorkflowActor");

            await proxy.RegisterReminder();

            return Success("OK");
        }

        [HttpGet("UnRegist/Reminder/{orderId}")]
        public async Task<ActionResult> UnregistReminderAsync(string orderId)
        {
            var actorId = new ActorId("reminder-actor-id-" + orderId);
            var proxy = ActorProxy.Create<IWorkFlowActor>(actorId, "WorkflowActor");
            await proxy.UnregisterReminder();
            return Success("OK");
        }

        #endregion

        #region Timer  

        [HttpGet("timer/{orderId}")]
        public async Task<ActionResult> TimerAsync(string orderId)
        {
            var actorId = new ActorId("timer-actor-id-" + orderId);
            var proxy = ActorProxy.Create<IWorkFlowActor>(actorId, "WorkflowActor");
            await proxy.RegisterTimer();
            return Success("OK");
        }

        [HttpGet("unregist/timer/{orderId}")]
        public async Task<ActionResult> UnregistTimerAsync(string orderId)
        {
            var actorId = new ActorId("timer-actor-id-" + orderId);
            var proxy = ActorProxy.Create<IWorkFlowActor>(actorId, "WorkflowActor");
            await proxy.UnregisterTimer();
            return Success("OK");
        }
        #endregion



    }
}
