using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();


app.Run();
