namespace ProdutorRuralSensores.Application.DTOs.Request;

/// <summary>
/// DTO para criação de uma nova leitura de sensor (dados IoT)
/// </summary>
public class LeituraCreateRequest
{
    /// <summary>
    /// ID do talhão onde a leitura foi realizada
    /// </summary>
    public Guid TalhaoId { get; set; }

    /// <summary>
    /// ID do sensor que realizou a leitura (opcional se usar código)
    /// </summary>
    public Guid? SensorId { get; set; }

    /// <summary>
    /// Código do sensor (alternativa ao SensorId)
    /// </summary>
    public string? CodigoSensor { get; set; }

    /// <summary>
    /// Umidade do solo em percentual (0-100)
    /// </summary>
    public decimal? UmidadeSolo { get; set; }

    /// <summary>
    /// Temperatura em Celsius
    /// </summary>
    public decimal? Temperatura { get; set; }

    /// <summary>
    /// Precipitação em mm
    /// </summary>
    public decimal? Precipitacao { get; set; }

    /// <summary>
    /// Umidade do ar em percentual (0-100)
    /// </summary>
    public decimal? UmidadeAr { get; set; }

    /// <summary>
    /// Velocidade do vento em km/h
    /// </summary>
    public decimal? VelocidadeVento { get; set; }

    /// <summary>
    /// Direção do vento (N, S, E, W, NE, etc.)
    /// </summary>
    public string? DirecaoVento { get; set; }

    /// <summary>
    /// Radiação solar em W/m²
    /// </summary>
    public decimal? RadiacaoSolar { get; set; }

    /// <summary>
    /// Pressão atmosférica em hPa
    /// </summary>
    public decimal? PressaoAtmosferica { get; set; }

    /// <summary>
    /// Data/hora da leitura (se não informada, usa a data atual)
    /// </summary>
    public DateTime? DataHoraLeitura { get; set; }
}
