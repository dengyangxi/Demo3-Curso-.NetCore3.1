using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();


app.Run();
