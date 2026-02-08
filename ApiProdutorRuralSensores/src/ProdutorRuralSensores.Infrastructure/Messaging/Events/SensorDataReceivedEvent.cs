namespace ProdutorRuralSensores.Infrastructure.Messaging.Events;

/// <summary>
/// Evento publicado quando uma leitura de sensor é recebida
/// Consumido pelo serviço de Monitoramento para processamento de alertas
/// </summary>
public record SensorDataReceivedEvent
{
    /// <summary>
    /// ID único do evento
    /// </summary>
    public Guid EventId { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Data/hora do evento
    /// </summary>
    public DateTime EventDateTime { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// ID da leitura registrada
    /// </summary>
    public Guid LeituraId { get; init; }

    /// <summary>
    /// ID do talhão
    /// </summary>
    public Guid TalhaoId { get; init; }

    /// <summary>
    /// ID do sensor (opcional)
    /// </summary>
    public Guid? SensorId { get; init; }

    /// <summary>
    /// Código do sensor
    /// </summary>
    public string? CodigoSensor { get; init; }

    /// <summary>
    /// Umidade do solo em percentual (0-100)
    /// </summary>
    public decimal? UmidadeSolo { get; init; }

    /// <summary>
    /// Temperatura em Celsius
    /// </summary>
    public decimal? Temperatura { get; init; }

    /// <summary>
    /// Precipitação em mm
    /// </summary>
    public decimal? Precipitacao { get; init; }

    /// <summary>
    /// Umidade do ar em percentual (0-100)
    /// </summary>
    public decimal? UmidadeAr { get; init; }

    /// <summary>
    /// Velocidade do vento em km/h
    /// </summary>
    public decimal? VelocidadeVento { get; init; }

    /// <summary>
    /// Radiação solar em W/m²
    /// </summary>
    public decimal? RadiacaoSolar { get; init; }

    /// <summary>
    /// Pressão atmosférica em hPa
    /// </summary>
    public decimal? PressaoAtmosferica { get; init; }

    /// <summary>
    /// Data/hora da leitura do sensor
    /// </summary>
    public DateTime DataHoraLeitura { get; init; }
}
