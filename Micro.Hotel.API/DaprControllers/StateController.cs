using Dapr;
using Dapr.Client;
using Micro.Common.Library;
using Micro.Common.Library.DaprStateStore;
using Microsoft.AspNetCore.Mvc;

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
 * 5. 状态管理 (State Management):
        独立的状态管理，使用键/值对作为存储机制，可以轻松的使长时运行、高可用的有状态服务和无状态服务共同运行在您的应用程序中。
        状态存储是可插拔的，目前支持使用Azure CosmosDB、 Azure SQL Server、 PostgreSQL,、AWS DynamoDB、Redis 作为状态存储介质。
 */

namespace Micro.Hotel.API.DaprControllers
{
    /// <summary>
    /// 5. State Management         状态管理
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
    public class StateController : MicroBaseAPIController
    {
        #region 0. DI 注入对象

        /// <summary>
        /// 日志对象
        /// </summary>
        private readonly ILogger<StateController> _logger;

        /// <summary>
        ///   Dapr 边车客户端
        ///       注： DaprClient 并非代码入侵试， 它只是将一些通用的http 的各种 请求封装成一个方法。
        ///            您也可以使用： DaprClient   发起 http / Grpc 请求
        /// </summary>
        private readonly DaprClient _daprClient;

        #endregion 0. DI 注入对象

        #region 0. 构造函数

        /// <summary>
        /// 构造函数
        ///       DI—Dependency Injection  依赖注入
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="daprClient"></param>
        public StateController(ILogger<StateController> logger, DaprClient daprClient)
        {
            //日志对象
            _logger = logger;

            //Dapr 客户端
            _daprClient = daprClient;
        }

        #endregion 0. 构造函数

        /// <summary>
        /// 保存一个state值
        /// </summary>
        /// <returns></returns>
        [HttpPost("SavaOrderStatus")]
        [HttpGet("SavaOrderStatus")]
        public async Task<ActionResult> SavaOrderStatusByStrongAsync(string key, string values)
        {
            //存储状态
            await _daprClient.SaveStateAsync<string>(
                    StateComponent.StateRedisComponentName, // 状态管理 组件 名称。即： 1.redis.statestore.yaml  metadata:  name: redis-statestore-component
                    key,
                    values,
                    new StateOptions()
                    {
                        //状态操作的一致性模式。  Eventual 最终一致性，   Strong 强一致性(如果redis 是集群 那么需要等待所有副本保存成功)
                        Consistency = ConsistencyMode.Strong,

                        //状态操作的并发模式  LastWrite 以最后一次为准，  FirstWrite 以第一次为准
                        Concurrency = ConcurrencyMode.LastWrite
                    }
                );
            return Success("成功");
        }

        /// <summary>
        /// 通过etag防止并发冲突，保存一个值
        /// </summary>
        /// <returns></returns>
        [HttpPost("SavaOrderStatusETag")]
        [HttpGet("SavaOrderStatusETag")]
        public async Task<ActionResult> SavaOrderStatusETagAsync(string key, string values)
        {
            //从Dapr状态存储和ETag中获取与键关联的当前值。
            var (value, etag) = await _daprClient.GetStateAndETagAsync<string>(
                    StateComponent.StateRedisComponentName,      // 状态管理 组件 名称。即： 1.redis.statestore.yaml  metadata:  name: redis-statestore-component
                    key,
                    ConsistencyMode.Strong
                );
            // 通过Etag 保存一个值
            var result = await _daprClient.TrySaveStateAsync<string>(StateComponent.StateRedisComponentName, key, values, etag);

            return result ? Success($"ETag: {etag}, value :{value}") : Failed<string>($"Etag: {etag}, value :{value}");
        }

        /// <summary>
        /// 获取一个值
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetOrderStatus")]
        [HttpPost("GetOrderStatus")]
        public async Task<ActionResult> GetAsync(string key)
        {
            var result = await _daprClient.GetStateAsync<string>(
                                    StateComponent.StateRedisComponentName,      // 状态管理 组件 名称。即： 1.redis.statestore.yaml  metadata:  name: redis-statestore-component
                                    key);

            //将result转换成json

            return Ok(result);
        }

        /// <summary>
        ///  获取一个值和ETag
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpGet("GetOrderStatusETag")]
        public async Task<ActionResult> GetOrderStatusETagAsync(string key)
        {
            var (value, etag) = await _daprClient.GetStateAndETagAsync<string>(StateComponent.StateRedisComponentName, key);

            return Success($"ETag: {etag}, value :{value}");
        }

        /// <summary>
        /// 删除一个值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpDelete("DeleteState")]
        public async Task<ActionResult> DeleteStateAsync(string key)
        {
            await _daprClient.DeleteStateAsync(StateComponent.StateRedisComponentName, key);
            return Success("OK");
        }

        /// <summary>
        /// 通过etag防止并发冲突，删除一个值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [HttpDelete("DeleteStateETag")]
        public async Task<ActionResult> DeleteStateETagAsync(string key)
        {
            var (value, etag) = await _daprClient.GetStateAndETagAsync<string>(StateComponent.StateRedisComponentName, key);

            var result = await _daprClient.TryDeleteStateAsync(StateComponent.StateRedisComponentName, key, etag);

            return result ? Success($"ETag: {etag}, value :{value}") : Failed<string>($"Etag: {etag}, value :{value}");
        }

        /// <summary>
        /// 根据FromState获取一个值，健值name从路由模板获取
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        [HttpGet("GetState/{name}")]
        public ActionResult GetFromBindingAsync([FromState(StateComponent.StateRedisComponentName, "pms")] StateEntry<string> state)
        {
            return Ok(state.Value);
        }

        // 根据FromState获取并修改值，健值name从路由模板获取
        [HttpPost("SetState/{name}")]
        public async Task<ActionResult> PostWithBindingAsync([FromState(StateComponent.StateRedisComponentName, "pms")] StateEntry<string> state)
        {
            state.Value = Guid.NewGuid().ToString();
            return Ok(await state.TrySaveAsync());
        }

        /// <summary>
        /// 获取多个个值
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetStateList")]
        public async Task<ActionResult> GetStateListAsync(List<string> keyList)
        {
            var result = await _daprClient.GetBulkStateAsync(StateComponent.StateRedisComponentName, keyList, 10);

            return Success(result);
        }

        /// <summary>
        /// 传递多个值，逐个删除
        /// </summary>
        /// <returns></returns>
        [HttpPost("DeleteStateList")]
        public async Task<ActionResult> DeleteStateListAsync(List<string> keyList)
        {
            //批量获取State ,
            var data = await _daprClient.GetBulkStateAsync(StateComponent.StateRedisComponentName, keyList, 10);
            var removeList = new List<BulkDeleteStateItem>();
            foreach (var item in data)
            {
                removeList.Add(new BulkDeleteStateItem(item.Key, item.ETag));
            }
            //批量删除
            await _daprClient.DeleteBulkStateAsync(StateComponent.StateRedisComponentName, removeList);

            return Success("OK");
        }
    }
}