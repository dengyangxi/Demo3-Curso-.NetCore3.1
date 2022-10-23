namespace Micro.Common.Library.DaprStateStore
{
    public class StateComponent
    {
        /// <summary>
        /// PubSub 组件名称。
        ///     即： 1.redis.statestore.yaml 文件内的： metadata: name: redis-statestore-component
        /// </summary>
        public const string StateRedisComponentName = "redis-statestore-component";
    }
}