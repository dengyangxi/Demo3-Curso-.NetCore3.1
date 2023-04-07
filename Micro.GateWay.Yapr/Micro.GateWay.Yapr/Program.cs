using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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

//builder.Configuration.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json");

// 注册： 控制器  和 注册  DAPR  
builder.Services.AddControllers().AddDapr();

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

