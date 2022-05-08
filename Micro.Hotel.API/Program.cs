using Microsoft.AspNetCore.Hosting;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using System.Net;


var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("program init main ...初始化...ing ");
logger.Error("program init main ...初始化...ing ");
logger.Warn("program init main ...初始化...ing ");

try
{
    var builder = WebApplication.CreateBuilder(args);


    // 将服务添加到容器中。
    builder.Services.AddControllersWithViews();

    // NLog：为依赖注入设置NLog
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
    builder.Host.UseNLog();


    //使用WebHost
    builder.WebHost.UseKestrel(webhost =>
        {
        //实现对特定端口的监控
            webhost.ListenAnyIP(5005);

        //↓↓↓↓↓↓↓↓↓↓ 各种资源限制↓↓↓↓↓↓↓↓↓↓

        //最大并发连接。如果设置为Null（默认值），意味着不作限制。
            webhost.Limits.MaxConcurrentConnections = 500;

        //可升级连接（比如从HTTP升级到WebSocket）的最大并发数。如果设置为Null（默认值），意味着不作限制。
            webhost.Limits.MaxConcurrentUpgradedConnections = 500;

        //请求主体最大字节数，默认值为30,000,000 字节（约28.6M）。如果设置为Null，意味着不作限制。
            webhost.Limits.MaxRequestBodySize = (1024 * 100); //修改成 100kb

        //接收请求报头的超时时间，默认为30秒。
            webhost.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(35);  //修改成 35秒

        // 响应缓冲区最大容量，默认值为65,536（64kb）。  
            webhost.Limits.MaxResponseBufferSize = (1024 * 100);  //修改成 100kb


        // 其他各种限制  . . . . . . . .

        });



    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();



    var app = builder.Build();

    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    app.UseSwagger();
    app.UseSwaggerUI();
    //}

    app.UseStaticFiles();



    app.UseAuthorization();

    app.MapControllers();


    app.Run();

    //app.Run(async (context) =>
    //{
    //    //设置一下默认 返回信息
    //    await context.Response.WriteAsync($"Hello Welcome to PMS Hotel API !~  Server Time : {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}。    ---- From  Hotel Api ");
    //});


}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception。 由于异常而停止了程序"); 

    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    //确保在应用程序退出之前刷新并停止内部计时器/线程（避免Linux上出现分段错误）
    NLog.LogManager.Shutdown();
}