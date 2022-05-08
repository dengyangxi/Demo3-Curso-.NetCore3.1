using Microsoft.AspNetCore.Hosting;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using System.Net;


var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("program init main ...��ʼ��...ing ");
logger.Error("program init main ...��ʼ��...ing ");
logger.Warn("program init main ...��ʼ��...ing ");

try
{
    var builder = WebApplication.CreateBuilder(args);


    // ��������ӵ������С�
    builder.Services.AddControllersWithViews();

    // NLog��Ϊ����ע������NLog
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
    builder.Host.UseNLog();


    //ʹ��WebHost
    builder.WebHost.UseKestrel(webhost =>
        {
        //ʵ�ֶ��ض��˿ڵļ��
            webhost.ListenAnyIP(5005);

        //�������������������� ������Դ���ơ�������������������

        //��󲢷����ӡ��������ΪNull��Ĭ��ֵ������ζ�Ų������ơ�
            webhost.Limits.MaxConcurrentConnections = 500;

        //���������ӣ������HTTP������WebSocket������󲢷������������ΪNull��Ĭ��ֵ������ζ�Ų������ơ�
            webhost.Limits.MaxConcurrentUpgradedConnections = 500;

        //������������ֽ�����Ĭ��ֵΪ30,000,000 �ֽڣ�Լ28.6M�����������ΪNull����ζ�Ų������ơ�
            webhost.Limits.MaxRequestBodySize = (1024 * 100); //�޸ĳ� 100kb

        //��������ͷ�ĳ�ʱʱ�䣬Ĭ��Ϊ30�롣
            webhost.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(35);  //�޸ĳ� 35��

        // ��Ӧ���������������Ĭ��ֵΪ65,536��64kb����  
            webhost.Limits.MaxResponseBufferSize = (1024 * 100);  //�޸ĳ� 100kb


        // ������������  . . . . . . . .

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
    //    //����һ��Ĭ�� ������Ϣ
    //    await context.Response.WriteAsync($"Hello Welcome to PMS Hotel API !~  Server Time : {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}��    ---- From  Hotel Api ");
    //});


}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception�� �����쳣��ֹͣ�˳���"); 

    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    //ȷ����Ӧ�ó����˳�֮ǰˢ�²�ֹͣ�ڲ���ʱ��/�̣߳�����Linux�ϳ��ֶַδ���
    NLog.LogManager.Shutdown();
}