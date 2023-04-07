using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// WebHost  ���� Kestrel Web ������
//   ������ο��ٷ��ĵ��� https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/servers/kestrel/endpoints?view=aspnetcore-6.0 
builder.WebHost.UseKestrel(webhost =>
{
	//ʵ�ֶ��ض��˿ڵļ��
	// webhost.ListenAnyIP(5005);

	webhost.ConfigureHttpsDefaults(config =>
	{

		//����SSL֤��
		//   config.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.AllowCertificate;
	});

	//�������������������� ������Դ���ơ�������������������

	//��󲢷����ӡ��������ΪNull��Ĭ��ֵ������ζ�Ų������ơ�
	webhost.Limits.MaxConcurrentConnections = null;

	//���������ӣ������HTTP������WebSocket������󲢷������������ΪNull��Ĭ��ֵ������ζ�Ų������ơ�
	webhost.Limits.MaxConcurrentUpgradedConnections = null;
	 
	//������������ֽ�����Ĭ��ֵΪ30,000,000 �ֽڣ�Լ28.6M�����������ΪNull����ζ�Ų������ơ�
	webhost.Limits.MaxRequestBodySize = (1024 * 100); //�޸ĳ� 100kb

	//��������ͷ�ĳ�ʱʱ�䣬Ĭ��Ϊ30�롣
	webhost.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(35);  //�޸ĳ� 35��

	// ��Ӧ���������������Ĭ��ֵΪ65,536��64kb����  
	webhost.Limits.MaxResponseBufferSize = (1024 * 100);  //�޸ĳ� 100kb


	// ������������  . . . . . . . . 

});

//builder.Configuration.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json");

// ע�᣺ ������  �� ע��  DAPR  
builder.Services.AddControllers().AddDapr();

builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.MapReverseProxy();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

