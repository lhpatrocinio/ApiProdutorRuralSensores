using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace ProdutorRuralSensores.Infrastructure.Messaging;

/// <summary>
/// Configuração do RabbitMQ: exchanges, queues e bindings
/// </summary>
public class RabbitMqSetup : IAsyncDisposable
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMqSetup> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    // Exchange e Queue names
    public const string ExchangeName = "agro.events";
    public const string DeadLetterExchange = "agro.events.dlx";
    public const string SensorDataQueue = "monitoramento.sensor.data";
    public const string DeadLetterQueue = "agro.events.dlq";
    public const string SensorDataRoutingKey = "sensor.data.#";

    public RabbitMqSetup(IConfiguration configuration, ILogger<RabbitMqSetup> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<IChannel> GetChannelAsync()
    {
        if (_channel != null && _channel.IsOpen)
            return _channel;

        await InitializeAsync();
        return _channel!;
    }

    public async Task InitializeAsync()
    {
        try
        {
            var factory = new ConnectionFactory
            {
                HostName = _configuration["RabbitMQ:HostName"] ?? "localhost",
                Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = _configuration["RabbitMQ:UserName"] ?? "guest",
                Password = _configuration["RabbitMQ:Password"] ?? "guest",
                VirtualHost = _configuration["RabbitMQ:VirtualHost"] ?? "/"
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            // Configurar Dead Letter Exchange
            await _channel.ExchangeDeclareAsync(
                exchange: DeadLetterExchange,
                type: ExchangeType.Direct,
                durable: true);

            // Configurar Dead Letter Queue
            await _channel.QueueDeclareAsync(
                queue: DeadLetterQueue,
                durable: true,
                exclusive: false,
                autoDelete: false);

            await _channel.QueueBindAsync(
                queue: DeadLetterQueue,
                exchange: DeadLetterExchange,
                routingKey: "dead-letter");

            // Configurar Exchange principal (topic)
            await _channel.ExchangeDeclareAsync(
                exchange: ExchangeName,
                type: ExchangeType.Topic,
                durable: true);

            // Configurar Queue para SensorDataReceivedEvent com DLX
            var queueArgs = new Dictionary<string, object?>
            {
                { "x-dead-letter-exchange", DeadLetterExchange },
                { "x-dead-letter-routing-key", "dead-letter" },
                { "x-message-ttl", 86400000 } // 24 horas
            };

            await _channel.QueueDeclareAsync(
                queue: SensorDataQueue,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: queueArgs);

            await _channel.QueueBindAsync(
                queue: SensorDataQueue,
                exchange: ExchangeName,
                routingKey: SensorDataRoutingKey);

            _logger.LogInformation("RabbitMQ configurado com sucesso. Exchange: {Exchange}, Queue: {Queue}",
                ExchangeName, SensorDataQueue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao configurar RabbitMQ");
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel != null)
        {
            await _channel.CloseAsync();
            _channel.Dispose();
        }

        if (_connection != null)
        {
            await _connection.CloseAsync();
            _connection.Dispose();
        }
    }
}
