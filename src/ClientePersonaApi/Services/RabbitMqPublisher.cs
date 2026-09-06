using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace ClientePersonaApi.Services;

public class RabbitMqPublisher : IRabbitMqPublisher
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqPublisher> _logger;

    public RabbitMqPublisher(IConfiguration configuration, ILogger<RabbitMqPublisher> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public void PublishClientCreated(object message)
    {
        try
        {
            var host = _configuration["RabbitMQ:Host"] ?? "localhost";
            var factory = new ConnectionFactory() { HostName = host };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "cliente_eventos",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

            channel.BasicPublish(exchange: "",
                                 routingKey: "cliente_eventos",
                                 basicProperties: null,
                                 body: body);
            _logger.LogInformation("Evento de cliente publicado en RabbitMQ");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo conectar a RabbitMQ, operando en modo fallback local");
        }
    }
}
