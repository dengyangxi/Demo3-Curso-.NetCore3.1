using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#if !DEBUG
//Ö¸¶¨±©Â¶¶Ë¿Ú
builder.WebHost.ConfigureKestrel((context, serverOptions) =>
{
    //serverOptions.Listen(IPAddress.Loopback, 80);
       serverOptions.ListenLocalhost(80);
    //serverOptions.Listen(IPAddress.Loopback, 5001, listenOptions =>
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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
