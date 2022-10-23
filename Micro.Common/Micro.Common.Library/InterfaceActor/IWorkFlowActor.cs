//使用Actor模型
using Dapr.Actors;

//Actor模型是1973年提出的一个分布式并发编程模式
//在Actor模型中，Actor参与者是一个并发原语，简单来说，一个参与者就是一个工人，与进程或线程一样能够工作或处理任务。
//Actor模型的理念非常简单：万物皆Actor
//在Actor模型中主角是actor，类似一种worker。Actor彼此之间直接发送消息，不需要经过什么中介，消息是异步发送和处理的。
//在Actor模型中一切都是actor，所有逻辑或模块都可以看成是actor，通过不同Actor之间的消息传递实现模块之间的通信和交互。

namespace Micro.Common.Library.InterfaceActor
{
    /// <summary>
    ///  定义一个审批流程 的Actor模型
    ///
    /// </summary>
    public interface IWorkFlowActor : IActor
    {
        /// <summary>
        ///  订单退款 自动审批流程
        /// </summary>
        /// <returns></returns>
        Task<BaseModel<bool>> OrderRefundApprove();

        #region Timer

        /// <summary>
        /// 创建 Timer Actor
        /// </summary>
        /// <returns></returns>
        Task RegisterTimer();

        /// <summary>
        /// 注销 Timer Actor
        /// </summary>
        /// <returns></returns>
        Task UnregisterTimer();

        #endregion Timer

        #region Reminder

        /// <summary>
        ///  创建 Reminder Actor
        /// </summary>
        /// <returns></returns>
        Task RegisterReminder();

        /// <summary>
        /// 注销 Reminder Actor
        /// </summary>
        /// <returns></returns>
        Task UnregisterReminder();

        #endregion Reminder
    }
}