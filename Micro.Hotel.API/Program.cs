using Micro.Common.Library; 
using NLog;
using NLog.Web;
using System.Text;


//var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
   
//logger.Debug("program init main ...��ʼ��...ing ");
//logger.Error("program init main ...��ʼ��...ing ");
//logger.Warn("program init main ...��ʼ��...ing ");
try
{
    var builder = WebApplication.CreateBuilder(args);



    // ��������ӵ������С�
    builder.Services.AddControllersWithViews();

    // NLog��Ϊ����ע������NLog
    //builder.Logging.ClearProviders();
    //builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
    //builder.Host.UseNLog();

    //logger.Debug("program init main ...��ʼ��...ing ");
    //logger.Error("program init main ...��ʼ��...ing ");
    //logger.Warn("program init main ...��ʼ��...ing ");
    //logger.Trace("program init main ...��ʼ��...ing "); 
    //    // Enables HTTP/3
    //    listenOptions.Protocols = HttpProtocols.Http3;
    //    // Adds a TLS certificate to the endpoint
    //    listenOptions.UseHttps(httpsOptions =>
    //    {
    //        httpsOptions.ServerCertificate = LoadCertificate();
    //    });


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


    // ע�᣺ ������  �� ע��  DAPR  
    builder.Services.AddControllers().AddDapr();

    // ע�᣺ Grpc
    builder.Services.AddGrpc();


    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();


    var basePath = AppContext.BaseDirectory;

    //ע�������ļ�
    builder.Services.AddSingleton(new Appsettings(basePath));
    //�����������
    builder.Services.AddCors();

    //���ر��� �ַ�����ʽ
    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

    var app = builder.Build();

    app.UseHttpsRedirection();

    //���� ����
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


    //�м��ί����ӵ�Ӧ�ó��������ܵ���...
    app.Use((context, next) =>
    {
        //HttpRequest�����Ŀ��Ա���ζ�ȡ...
        context.Request.EnableBuffering();
        return next();
    });
    // ��������
    app.MapSubscribeHandler();

    //point.MapGrpcService<xxxxGrpc������>();
    // app.MapGrpcService<>

    app.Urls.Add("http://localhost:5005");



    app.Run(); 


}
catch (Exception exception)
{
    // NLog: catch setup errors
  //  logger.Error(exception, "Stopped program because of exception�� �����쳣��ֹͣ�˳���");

    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    //ȷ����Ӧ�ó����˳�֮ǰˢ�²�ֹͣ�ڲ���ʱ��/�̣߳�����Linux�ϳ��ֶַδ���
    //NLog.LogManager.Shutdown();
}