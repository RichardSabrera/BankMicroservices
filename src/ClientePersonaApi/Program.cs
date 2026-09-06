using ClientePersonaApi.Application.Services;
using ClientePersonaApi.Data;
using ClientePersonaApi.Domain.Interfaces;
using ClientePersonaApi.Infrastructure.Persistence;
using ClientePersonaApi.Infrastructure.Repositories;
using ClientePersonaApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? "Server=localhost;Database=DevSuDBCliente;User Id=sa;Password=123;TrustServerCertificate=True;";

if (builder.Environment.IsEnvironment("Testing"))
{
    builder.Services.AddDbContext<ClienteDbContext>(options =>
        options.UseInMemoryDatabase("TestDb_DevSuDBCliente"));
}
else
{
    builder.Services.AddDbContext<ClienteDbContext>(options =>
        options.UseSqlServer(connectionString));
}

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();

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
    var db = scope.ServiceProvider.GetRequiredService<ClienteDbContext>();
    try
    {
        if (!app.Environment.IsEnvironment("Testing"))
        {
            db.Database.EnsureCreated();
        }
        ClienteSeedData.Seed(db);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al inicializar la base de datos de Clientes: {ex.Message}");
    }
}

app.Run();

namespace ClientePersonaApi
{
    public partial class Program { }
}
