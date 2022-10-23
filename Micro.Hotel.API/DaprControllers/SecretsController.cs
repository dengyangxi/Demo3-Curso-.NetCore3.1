using Dapr.Client;
using Micro.Common.Library;
using Microsoft.AspNetCore.Mvc;

/*
 * Dapr :
    CNCF云原生基金会：  https://www.cncf.io/projects/dapr/
    Dapr源代码Github:   https://github.com/dapr/dapr
    Dapr官方中文文档：  https://docs.dapr.io/zh-hans/

    Dapr支持各语言SDK：
      1. 适用于 .NET Core  的 Dapr SDK: https://github.com/dapr/dotnet-sdk
      2. 适用于 Go         的 Dapr SDK: https://github.com/dapr/go-sdk
      3. 适用于 Java       的 Dapr SDK: https://github.com/dapr/java-sdk
      4. 适用于 JavaScript 的 Dapr SDK: https://github.com/dapr/js-sdk
      5. 适用于 Python     的 Dapr SDK: https://github.com/dapr/python-sdk
      6. 适用于 Rust       的 Dapr SDK: https://github.com/dapr/rust-sdk
      7. 适用于 C++        的 Dapr SDK: https://github.com/dapr/cpp-sdk
      8. 适用于 PHP        的 Dapr SDK: https://github.com/dapr/php-sdk

*/

/*
 * 3. 秘密管理 (Secret Management):
        Dapr 提供了密钥管理，支持与公有云和本地的Secret存储集成，以供应用检索使用。
 */

namespace Micro.Hotel.API.DaprControllers
{
    /// <summary>
    /// 3. Secret Management        秘密管理
    ///
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
    public class SecretsController : MicroBaseAPIController
    {
        #region 0. DI 注入对象

        /// <summary>
        /// 日志对象
        /// </summary>
        private readonly ILogger<SecretsController> _logger;

        /// <summary>
        ///   Dapr 边车客户端
        ///       注： DaprClient 并非代码入侵试， 它只是将一些通用的http 的各种 请求封装成一个方法。
        ///            您也可以使用： DaprClient   发起 http / Grpc 请求
        /// </summary>
        private readonly DaprClient _daprClient;

        #endregion 0. DI 注入对象



        public SecretsController(ILogger<SecretsController> logger, DaprClient daprClient)
        {
            _logger = logger;
            _daprClient = daprClient;
        }

        /// <summary>
        ///  存储组件
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> GetAsync()
        {
            Dictionary<string, string> secrets = await _daprClient.GetSecretAsync(
                                                    "local.secret.component",   //secret中间件的名字。  即： 1.localfile.secret.yaml 文件的  metadata:  name: local.secret.component
                                                     "MySecretKeyName"       //秘钥Key名称
                                                     );

            return Success(secrets["MySecretKeyName"]);
        }

        //[HttpGet("get01")]
        //public ActionResult Get01Async()
        //{
        //}
    }
}