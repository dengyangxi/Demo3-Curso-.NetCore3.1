using System.Net;
using System.Text;
using Micro.Common.Library;
using Micro.Order.API.Actors;
using Micro.Order.API.OrderService;
// Furion ע��
var builder = WebApplication.CreateBuilder(args).Inject();

// Add services to the container.

//ʹ��WebHost
builder.WebHost.UseKestrel(webhost =>
{
    //ʵ�ֶ��ض��˿ڵļ��
    //  webhost.ListenAnyIP(5006);
                                                           
    webhost.ConfigureHttpsDefaults(config =>
    {
        //����SSL֤��
        //   config.ClientCertificateMode = Microsoft.AspNetCore.Server.Kestrel.Https.ClientCertificateMode.AllowCertificate;
    });

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


// 1. ע�᣺ ������  �� ע��  DAPR  
// 2. ע��  Furion   AddInject
builder.Services.AddControllers().AddDapr().AddInject();

// ע��Զ���������
builder.Services.AddRemoteRequest();

// ע�᣺ Grpc
builder.Services.AddGrpc();

var basePath = AppContext.BaseDirectory;

//ע�������ļ�
builder.Services.AddSingleton(new Appsettings(basePath));

//�����������
builder.Services.AddCors();



builder.Services.AddActors(options =>
{
    options.Actors.RegisterActor<WorkflowActor>();
});



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//���ر��� �ַ�����ʽ
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


//ע���м��  Furion 
app.UseInject();

app.MapControllers();


// ��Dapr�� Actorsӳ�䵽�ܵ� Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.
app.MapActorsHandlers();


//�� �������� ӳ�䵽 Grpc ���񣬱�¶�ս�� Grpc Service Endpoint ��
//      IEndpointRouteBuilder  GrpcServiceEndpointConventionBuilder �� ���������
app.MapGrpcService<OrderService>();


//�м��ί����ӵ�Ӧ�ó��������ܵ���...
app.Use((context, next) =>
{
    //HttpRequest�����Ŀ��Ա���ζ�ȡ...
    context.Request.EnableBuffering();
    return next();
});



// ��������
app.MapSubscribeHandler();

app.Urls.Add("http://localhost:5006");

app.Run();
