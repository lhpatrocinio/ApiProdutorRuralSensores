using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using ProdutorRuralSensores.Infrastructure.Messaging.Events;
using RabbitMQ.Client;

namespace ProdutorRuralSensores.Infrastructure.Messaging;

/// <summary>
/// Interface para publicação de eventos de dados de sensores
/// </summary>
public interface ISensorDataPublisher
{
    Task PublishAsync(SensorDataReceivedEvent @event);
}

/// <summary>
/// Publicador de eventos de dados de sensores via RabbitMQ
/// </summary>
public class SensorDataPublisher : ISensorDataPublisher
{
    private readonly RabbitMqSetup _rabbitMqSetup;
    private readonly ILogger<SensorDataPublisher> _logger;

    public SensorDataPublisher(RabbitMqSetup rabbitMqSetup, ILogger<SensorDataPublisher> logger)
    {
        _rabbitMqSetup = rabbitMqSetup;
        _logger = logger;
    }

    public async Task PublishAsync(SensorDataReceivedEvent @event)
    {
        try
        {
            var channel = await _rabbitMqSetup.GetChannelAsync();

            var message = JsonSerializer.Serialize(@event);
            var body = Encoding.UTF8.GetBytes(message);

            var routingKey = $"sensor.data.{@event.TalhaoId}";

            var properties = new BasicProperties
            {
                Persistent = true,
                ContentType = "application/json",
                MessageId = @event.EventId.ToString(),
                Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            };

            await channel.BasicPublishAsync(
                exchange: RabbitMqSetup.ExchangeName,
                routingKey: routingKey,
                mandatory: false,
                basicProperties: properties,
                body: body);

            _logger.LogInformation(
                "Evento SensorDataReceivedEvent publicado. EventId: {EventId}, TalhaoId: {TalhaoId}, LeituraId: {LeituraId}",
                @event.EventId, @event.TalhaoId, @event.LeituraId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Erro ao publicar evento SensorDataReceivedEvent. TalhaoId: {TalhaoId}, LeituraId: {LeituraId}",
                @event.TalhaoId, @event.LeituraId);
            throw;
        }
    }
}
