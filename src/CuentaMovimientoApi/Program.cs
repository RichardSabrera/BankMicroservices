using CuentaMovimientoApi.Application.Services;
using CuentaMovimientoApi.Data;
using CuentaMovimientoApi.Infrastructure.Persistence;
using CuentaMovimientoApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? "Server=localhost;Database=DevSuDBMovimientos;User Id=sa;Password=123;TrustServerCertificate=True;";

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<CuentaDbContext>(options =>
        options.UseInMemoryDatabase("TestDb_DevSuDBMovimientos"));
}
else
{
    builder.Services.AddDbContext<CuentaDbContext>(options =>
        options.UseSqlServer(connectionString));
}

builder.Services.AddScoped<ICuentaService, CuentaService>();
builder.Services.AddScoped<IMovimientoService, MovimientoService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddHostedService<RabbitMqConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CuentaDbContext>();
    try
    {
        if (!app.Environment.IsEnvironment("Testing"))
        {
            db.Database.EnsureCreated();
        }
        CuentaSeedData.Seed(db);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al inicializar la base de datos de Cuentas: {ex.Message}");
    }
}

app.Run();

namespace CuentaMovimientoApi
{
    public partial class Program { }
}
