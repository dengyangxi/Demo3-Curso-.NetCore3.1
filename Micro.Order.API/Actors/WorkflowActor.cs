using Dapr.Actors.Runtime;
using Micro.Common.Library;
using Micro.Common.Library.Extensions;
using Micro.Common.Library.InterfaceActor;
using System.Text.Json;

namespace Micro.Order.API.Actors
{

    /*
     * 
     * 如何解决云生应用高并发的应用场景呢？  那首先我们需要明确处理高并发的难点在哪？
        第一个是高性能，第二个就是：资源竞争导致的数据一致性问题。
        1. 对于第一个难点，通过水平扩展服务可以化解；
        2. 对于第二个难点，一般就是采用锁机制，
        而对于分布式的应用场景下，处理手段就更加复杂，可能需要使用分布式锁，而这种做法，大大降低了应用的整体响应性能。
        那有没有更好的解决方案，既兼顾性能又可以确保数据一致性呢？

   A:  有，借助Actor模型 : 

    简单来讲：Actor模型 = 状态 + 行为 + 消息。
    一个应用/服务由多个Actor组成，每个Actor都是一个独立的运行单元，拥有隔离的运行空间，在隔离的空间内，其有独立的状态和行为，不被外界干预，Actor之间通过消息进行交互，
    而同一时刻，每个Actor只能被单个线程执行，这样既有效避免了数据共享和并发问题，又确保了应用的伸缩性。
    另外Actor基于事件驱动模型进行异步通信，性能良好。且位置透明，无论Actor是在本机亦或是在集群中的其他机器，都可以直接进行透明调用。
    因此Actor模型赋予了应用/服务的生命力（有状态）、高并发的处理能力和弹性伸缩能力。


    Actor模型于1970年年初被提出的
      几乎在所有主流开发平台下都有了Actor模型的实现：
                Java平台下Scala的Actor类库和Jetlang；
                NET平台下的MS CCR和Retlang； (另外，前些天听 张善友 （张队）介绍，  .net 7 里面会有更加丰富的 Actor模型 实现。 )
              

     Orleans 以支持高并发的分布式业务(内部也使用Actor)。github :  https://github.com/dotnet/orleans
        Orleans 框架中有一个模块，分布式事务强一致性。（ACID）：  https://docs.microsoft.com/zh-cn/dotnet/orleans/grains/transactions 
                   DotNetCore.CAP 分布式事务最终一致性。（CAP）:  https://github.com/dotnetcore/CAP
    */

    /// <summary>
    /// Actor 模型实现类
    /// </summary>
    public class WorkflowActor : Actor, IWorkFlowActor, IRemindable
    {
        /// <summary>
        /// 日志对象
        /// </summary>
        private readonly ILogger _logger;

        public WorkflowActor(ActorHost host, ILogger<WorkflowActor> logger) : base(host)
        {
            _logger = logger;
        }


        /*
         
              1. Timers 数据不会 持久化 到 State，即： 重启后就会失效
              2. Reminders  数据会持久化到 state, 即： 重启服务Actor依然有效
              3. 如果创建一个Actor 长时间不适用，会被回收掉，再次请求重新创建 Actor

         */

        /// <summary>
        ///  订单退款 自动审批流程
        ///       此方法采用 Actor 来调度处理，不管部署多少副本。相同的ActorID 将会单线程执行。
        /// </summary>
        /// <returns></returns>
        public async Task<BaseModel<bool>> OrderRefundApprove()
        {
            try
            {

                await StateManager.AddOrUpdateStateAsync(
                     Id.ToString(), //ActorID 
                    (new BaseModel<int>() { Msg = "init step: 0", Data = 1, IsError = false, Code = 100 }).ToJson(),      // 如果不存在，初始化的值
                    (actorID, currentStatus) =>     //如果存在，更新值
                        {
                            //actorID         :  Actor模型ID  
                            //currentStatus   :  上一次处理后的结果信息（当前未处理之前的信息）


                            // 根据 上一次的 currentStatus 信息...处理你的业务请求（如下举例）： 
                            var lastResult = currentStatus.ToModel<BaseModel<int>>();
                            if (lastResult.IsError == false)
                            {
                                return (new BaseModel<int>() { Msg = $" step: {(lastResult.Data + 1)} ", Data = (lastResult.Data + 1), IsError = false, Code = (lastResult.Code + 1) }).ToJson();
                            }
                            return (new BaseModel<int>() { Msg = $" step: ", Data = (lastResult.Data + 1), IsError = true, Code = (lastResult.Code + 1) }).ToJson();
                        }
                    );

                return new BaseModel<bool>() { Msg = $"ActorID:{Id.ToString()}", IsError = false, Data = true };


                //1.   "{\"Data\":0,\"IsError\":false,\"Msg\":\"init step: 0",\"Code\":100}"
                //2.   "{\"Data\":1,\"IsError\":false,\"Msg\":\" step: 1",\"Code\":101}"
                //3.   "{\"Data\":2,\"IsError\":false,\"Msg\":\" step: 2",\"Code\":102}"
                //4.   "{\"Data\":3,\"IsError\":false,\"Msg\":\" step: 3",\"Code\":103}"
            }
            catch (Exception ex)
            {
                return new BaseModel<bool>() { Msg = $"ActorID:{Id.ToString()},ErrorMsg:{ex.Message}", IsError = true, Data = false, Code = 500 };
            }
        }



        #region 一、 Reminder  （会持久化）



        /// <summary>
        /// 注册一个 Reminder
        /// </summary>
        /// <returns></returns>
        public async Task RegisterReminder()
        {
            //提醒消息内容
            var state = JsonSerializer.SerializeToUtf8Bytes
                                            (
                                                (new BaseModel<string>() { Msg = "", Code = 200, Data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), IsError = false }).ToJson()
                                            );

            await this.RegisterReminderAsync(
                                    "dapr.actor.reminder",          //名称
                                    state,                         // 提醒消息内容
                                    TimeSpan.FromSeconds(3),       //首次调用提醒之前的延迟时间:  -1/毫秒  不需要提醒， 0 立即提醒 ， X / 后提醒 （延时）
                                    TimeSpan.FromSeconds(3)        //第一次调用提醒之后，每隔多少时间调用（即：第二次，第三次，第四次...调用间隔时间）。 -1/毫秒 禁用定期调用 。
                                    );

        }

        /// <summary>
        ///  提醒回调
        ///     实现接口 IRemindable 的方法 ReceiveReminderAsync
        /// </summary>
        /// <param name="reminderName"></param>
        /// <param name="state"></param>
        /// <param name="dueTime"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {

            var json = JsonSerializer.Deserialize<string>(state);

            _logger.LogInformation(message: $"ReceiveReminderAsync String:{json}");
            if (json is null)
            {
                json = (new BaseModel<string>() { IsError = true, Msg = "json is null" }).ToJson();
            }
            var model = json.ToModel<BaseModel<string>>();


            _logger.LogInformation(message: $"ReceiveReminderAsync Json:{model.ToJson()}");


            await this.StateManager.SetStateAsync<string>(Id.ToString(), model.ToJson());

        }

        /// <summary>
        /// 注销提醒
        /// </summary>
        /// <returns></returns>
        public Task UnregisterReminder()
        {
            return this.UnregisterReminderAsync("dapr.actor.reminder");
        }

        #endregion 



        #region 二、 Timer   （非持久化）

        public Task RegisterTimer()
        {
            //提醒消息内容
            var state = JsonSerializer.SerializeToUtf8Bytes
                                            (
                                                (new BaseModel<string>() { Msg = "ActorID:dapr.actor.timer 的模型消息", Code = 200, Data = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), IsError = false }).ToJson()
                                            );


            return this.RegisterTimerAsync(
                "dapr.actor.timer",               //名称
                nameof(this.TimerCallBack),       //回调方法名称
                state,                            // 提醒消息内容
                TimeSpan.FromSeconds(3),          //首次调用提醒之前的延迟时间:  -1/毫秒  不需要提醒， 0 立即提醒 ， X / 后提醒 （延时）
                TimeSpan.FromSeconds(3)           //第一次调用提醒之后，每隔多少时间调用（即：第二次，第三次，第四次...调用间隔时间）。 -1/毫秒 禁用定期调用 。
                );





        }

        /// <summary>
        /// 回调方法
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task TimerCallBack(byte[] state)
        {
            var json = JsonSerializer.Deserialize<string>(state);

            _logger.LogInformation(message: $"ReceiveReminderAsync String:{json}");
            if (json is null)
            {
                json = (new BaseModel<string>() { IsError = true, Msg = "json is null" }).ToJson();
            }
            var model = json.ToModel<BaseModel<string>>();


            _logger.LogInformation(message: $"ReceiveReminderAsync Json:{model.ToJson()}");

            await this.StateManager.SetStateAsync<string>(Id.ToString(), model.ToJson());
        }

        /// <summary>
        /// 注销提醒
        /// </summary>
        /// 
        /// <returns></returns>
        public Task UnregisterTimer()
        {
            return this.UnregisterTimerAsync("dapr.actor.timer");
        }
        #endregion  

    }
}
