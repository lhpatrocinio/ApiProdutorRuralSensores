namespace ProdutorRuralSensores.Application.DTOs.Request;

/// <summary>
/// DTO para criação de um novo sensor
/// </summary>
public class SensorCreateRequest
{
    /// <summary>
    /// ID do talhão onde o sensor está instalado
    /// </summary>
    public Guid TalhaoId { get; set; }

    /// <summary>
    /// Código único do sensor
    /// </summary>
    public string Codigo { get; set; } = string.Empty;

    /// <summary>
    /// Tipo do sensor (Umidade, Temperatura, Multiparâmetro)
    /// </summary>
    public string Tipo { get; set; } = string.Empty;

    /// <summary>
    /// Modelo do sensor
    /// </summary>
    public string? Modelo { get; set; }

    /// <summary>
    /// Fabricante do sensor
    /// </summary>
    public string? Fabricante { get; set; }

    /// <summary>
    /// Data de instalação do sensor
    /// </summary>
    public DateTime? DataInstalacao { get; set; }

    /// <summary>
    /// Latitude da localização do sensor
    /// </summary>
    public decimal? Latitude { get; set; }

    /// <summary>
    /// Longitude da localização do sensor
    /// </summary>
    public decimal? Longitude { get; set; }
}
