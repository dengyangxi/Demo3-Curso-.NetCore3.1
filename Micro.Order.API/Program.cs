using System.Net;
using System.Text;
using Micro.Common.Library;
using Micro.Order.API.Actors;
using Micro.Order.API.OrderService;
// Furion 注入
var builder = WebApplication.CreateBuilder(args).Inject();

// Add services to the container.

//使用WebHost
builder.WebHost.UseKestrel(webhost =>
{
    //实现对特定端口的监控
    //  webhost.ListenAnyIP(5006);
                                                           
    webhost.ConfigureHttpsDefaults(config =>
    {
        //忽略SSL证书
        //   config.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.AllowCertificate;
    });

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


// 1. 注册： 控制器  和 注册  DAPR  
// 2. 注册  Furion   AddInject
builder.Services.AddControllers().AddDapr().AddInject();

// 注册远程请求服务
builder.Services.AddRemoteRequest();

// 注册： Grpc
builder.Services.AddGrpc();

var basePath = AppContext.BaseDirectory;

//注入配置文件
builder.Services.AddSingleton(new Appsettings(basePath));

//启动跨域策略
builder.Services.AddCors();



builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<WorkflowActor>();
});



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//加载编码 字符串格式
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


var app = builder.Build();


// Configure the HTTP request pipeline.



app.UseHttpsRedirection();


//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseAuthorization();


//注册中间件  Furion 
app.UseInject();

app.MapControllers();


// 将Dapr的 Actors映射到管道 Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.
app.MapActorsHandlers();


//将 订单服务 映射到 Grpc 服务，暴露终结点 Grpc Service Endpoint 。
//      IEndpointRouteBuilder  GrpcServiceEndpointConventionBuilder 与 服务关联。
app.MapGrpcService<OrderService>();


//中间件委托添加到应用程序的请求管道中...
app.Use((context, next) =>
{
    //HttpRequest。正文可以被多次读取...
    context.Request.EnableBuffering();
    return next();
});



// 开启订阅
app.MapSubscribeHandler();

app.Urls.Add("http://localhost:5006");

app.Run();
