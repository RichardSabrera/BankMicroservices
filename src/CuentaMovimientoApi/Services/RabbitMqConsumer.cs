using System.Text;
using System.Text.Json;
using CuentaMovimientoApi.Domain.Entities;
using CuentaMovimientoApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CuentaMovimientoApi.Services;

public class RabbitMqConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqConsumer> _logger;

    public RabbitMqConsumer(IServiceProvider serviceProvider, IConfiguration configuration, ILogger<RabbitMqConsumer> logger)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.Run(() => StartListening(stoppingToken), stoppingToken);
    }

    private void StartListening(CancellationToken stoppingToken)
    {
        try
        {
            var host = _configuration["RabbitMQ:Host"] ?? "localhost";
            var factory = new ConnectionFactory() { HostName = host };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "cliente_eventos",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation("Mensaje recibido de RabbitMQ: {Message}", message);

                try
                {
                    using var doc = JsonDocument.Parse(message);
                    var root = doc.RootElement;

                    int id = root.TryGetProperty("Id", out var idProp) ? idProp.GetInt32() : 0;
                    string clienteId = root.GetProperty("ClienteId").GetString()!;
                    string nombre = root.GetProperty("Nombre").GetString()!;
                    string identificacion = root.TryGetProperty("Identificacion", out var idenProp) ? idenProp.GetString()! : "";
                    bool estado = root.TryGetProperty("Estado", out var stProp) ? stProp.GetBoolean() : true;

                    using var scope = _serviceProvider.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<CuentaDbContext>();

                    var existing = await db.Clientes.FirstOrDefaultAsync(c => (id > 0 && c.Id == id) || c.ClienteId == clienteId, stoppingToken);
                    if (existing == null)
                    {
                        var newClient = new ClienteReadModel
                        {
                            ClienteId = clienteId,
                            Nombre = nombre,
                            Identificacion = identificacion,
                            Estado = estado
                        };
                        if (id > 0) newClient.Id = id;

                        db.Clientes.Add(newClient);
                    }
                    else
                    {
                        existing.ClienteId = clienteId;
                        existing.Nombre = nombre;
                        existing.Identificacion = identificacion;
                        existing.Estado = estado;
                    }
                    await db.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesar mensaje de cliente en RabbitMQ Consumer");
                }
            };

            channel.BasicConsume(queue: "cliente_eventos", autoAck: true, consumer: consumer);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RabbitMQ Consumer operando en modo pasivo local");
        }
    }
}
