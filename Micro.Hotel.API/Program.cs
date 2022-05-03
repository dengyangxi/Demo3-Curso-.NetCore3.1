using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#if !DEBUG
//ָ����¶�˿�
builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{

    //�����˿� 80 
    serverOptions.ListenLocalhost(5005);

    //serverOptions.Listen(IPAddress.Loopback, 80);
    //serverOptions.Listen(IPAddress.Loopback, 80, listenOptions =>
    //{
    //    listenOptions.UseHttps("testCert.pfx", "testPassword");
    //});
});

#endif

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

app.UseAuthorization();

app.MapControllers();


app.Run();

//app.Run(async (context) =>
//{
//    //����һ��Ĭ�� ������Ϣ
//    await context.Response.WriteAsync($"Hello Welcome to PMS Hotel API !~  Server Time : {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}��    ---- From  Hotel Api ");
//});
