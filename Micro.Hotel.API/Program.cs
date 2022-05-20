using Micro.Common.Library; 
using NLog;
using NLog.Web;
using System.Text;


//var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
   
//logger.Debug("program init main ...初始化...ing ");
//logger.Error("program init main ...初始化...ing ");
//logger.Warn("program init main ...初始化...ing ");
try
{
    var builder = WebApplication.CreateBuilder(args);



    // 将服务添加到容器中。
    builder.Services.AddControllersWithViews();

    // NLog：为依赖注入设置NLog
    //builder.Logging.ClearProviders();
    //builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
    //builder.Host.UseNLog();

    //logger.Debug("program init main ...初始化...ing ");
    //logger.Error("program init main ...初始化...ing ");
    //logger.Warn("program init main ...初始化...ing ");
    //logger.Trace("program init main ...初始化...ing "); 
    //    // Enables HTTP/3
    //    listenOptions.Protocols = HttpProtocols.Http3;
    //    // Adds a TLS certificate to the endpoint
    //    listenOptions.UseHttps(httpsOptions =>
    //    {
    //        httpsOptions.ServerCertificate = LoadCertificate();
    //    });


    // WebHost  设置 Kestrel Web 服务器
    //   详情请参考官方文档： https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-6.0 
    builder.WebHost.UseKestrel(webhost =>
        {
            //实现对特定端口的监控
            // webhost.ListenAnyIP(5005);

            webhost.ConfigureHttpsDefaults(config =>
            {

                //忽略SSL证书
                //   config.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.AllowCertificate;
            });

            //↓↓↓↓↓↓↓↓↓↓ 各种资源限制↓↓↓↓↓↓↓↓↓↓

            //最大并发连接。如果设置为Null（默认值），意味着不作限制。
            webhost.Limits.MaxConcurrentConnections = null;

            //可升级连接（比如从HTTP升级到WebSocket）的最大并发数。如果设置为Null（默认值），意味着不作限制。
            webhost.Limits.MaxConcurrentUpgradedConnections = null;

            //请求主体最大字节数，默认值为30,000,000 字节（约28.6M）。如果设置为Null，意味着不作限制。
            webhost.Limits.MaxRequestBodySize = (1024 * 100); //修改成 100kb

            //接收请求报头的超时时间，默认为30秒。
            webhost.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(35);  //修改成 35秒

            // 响应缓冲区最大容量，默认值为65,536（64kb）。  
            webhost.Limits.MaxResponseBufferSize = (1024 * 100);  //修改成 100kb


            // 其他各种限制  . . . . . . . .

        });


    // 注册： 控制器  和 注册  DAPR  
    builder.Services.AddControllers().AddDapr();

    // 注册： Grpc
    builder.Services.AddGrpc();


    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();


    var basePath = AppContext.BaseDirectory;

    //注入配置文件
    builder.Services.AddSingleton(new Appsettings(basePath));
    //启动跨域策略
    builder.Services.AddCors();

    //加载编码 字符串格式
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    var app = builder.Build();

    app.UseHttpsRedirection();

    //跨域 设置
    app.UseCors(Appsettings.app(new string[] { "Startup", "Cors", "PolicyName" }));

    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    app.UseSwagger();

    app.UseSwaggerUI();
    //}

    app.UseStaticFiles();



    app.UseAuthorization();

    app.MapControllers();


    //中间件委托添加到应用程序的请求管道中...
    app.Use((context, next) =>
    {
        //HttpRequest。正文可以被多次读取...
        context.Request.EnableBuffering();
        return next();
    });
    // 开启订阅
    app.MapSubscribeHandler();

    //point.MapGrpcService<xxxxGrpc服务类>();
    // app.MapGrpcService<>

    app.Urls.Add("http://localhost:5005");



    app.Run(); 


}
catch (Exception exception)
{
    // NLog: catch setup errors
  //  logger.Error(exception, "Stopped program because of exception。 由于异常而停止了程序");

    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    //确保在应用程序退出之前刷新并停止内部计时器/线程（避免Linux上出现分段错误）
    //NLog.LogManager.Shutdown();
}