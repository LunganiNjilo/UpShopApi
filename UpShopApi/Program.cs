using UpShopApi.Application.Interfaces;
using UpShopApi.Application.Services;
using UpShopApi.Domain.Models;
using UpShopApi.Extensions;
using UpShopApi.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173") // your frontend port
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


// Remove HTTPS entirely for Docker
// Detect container
var isContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

if (isContainer)
{
    builder.WebHost.ConfigureKestrel(options =>
    {
        options.ListenAnyIP(8080); // HTTP only in container
    });
}

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? (isContainer
        ? "/app/sql/UpShop.db"      // absolute path inside container
        : "Infrastructure/sql/UpShop.db"); // local dev fallback

builder.Services.AddScoped<IRepository<Product>>(sp =>
    new GenericRepository<Product>(connectionString, "tb_Products"));

builder.Services.AddCustomErrorHandling();
builder.Services.AddScoped<IProductService, ProductService>();
var app = builder.Build();

app.UseCors();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseGlobalExceptionHandling();
app.MapControllers();

app.Run();

public partial class Program { }