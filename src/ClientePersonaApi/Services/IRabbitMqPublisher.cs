namespace ClientePersonaApi.Services;

public interface IRabbitMqPublisher
{
    void PublishClientCreated(object message);
}
