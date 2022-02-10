using System.Net;
using System.Security.Cryptography.X509Certificates;
using OnlineStoreAPI.Interfaces;
using OnlineStoreAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.WebHost.UseKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Any, 5000, listenOptions =>
    {
        listenOptions.UseHttps(new X509Certificate2("cert.pfx", "Passord01"));
    });
});

builder.Services.AddCors(o => o.AddPolicy("CorsPolicy", b =>
{
    b.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHsts();
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();