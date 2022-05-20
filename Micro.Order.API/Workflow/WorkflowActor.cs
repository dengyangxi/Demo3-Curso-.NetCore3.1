using Dapr.Actors.Runtime;
using Micro.Common.Library.InterfaceActor;
using System.Text.Json;

namespace Micro.Order.API.Workflow
{
    public class WorkflowActor : Actor, IWorkFlowActor, IRemindable
    {
        private readonly ILogger _logger;
        public WorkflowActor(ActorHost host, ILogger<WorkflowActor> logger) : base(host)
        {
            _logger = logger;
        }

        public async Task<bool> Approve()
        {
            await StateManager.AddOrUpdateStateAsync(Id.ToString(), "approve", (key, currentStatus) => "approve");
            return true;
        }

        public async Task ReceiveReminderAsync(string reminderName, byte[] state, TimeSpan dueTime, TimeSpan period)
        {
            var stateKey = "nowtime";
            var content = "now is " + DateTime.Now.ToString();
            _logger.LogInformation($" reminder---------{content}");
            await this.StateManager.SetStateAsync<string>(stateKey, content);

        }

        public async Task RegisterReminder()
        {
            await this.RegisterReminderAsync("TestReminder", null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5));

        }
        public Task UnregisterReminder()
        {
            return this.UnregisterReminderAsync("TestReminder");
        }



        public Task RegisterTimer()
        {
            var datetime = JsonSerializer.SerializeToUtf8Bytes(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            return this.RegisterTimerAsync("TestTimer", nameof(this.TimerCallback), datetime, TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(3));
        }

        public async Task TimerCallback(byte[] data)
        {
            var stateKey = "nowtime";
            var content = JsonSerializer.Deserialize<string>(data);
            _logger.LogInformation(" ---------" + content);
            await this.StateManager.SetStateAsync<string>(stateKey, content);
        }


        public Task UnregisterTimer()
        {
            return this.UnregisterTimerAsync("TestTimer");
        }
    }
}
