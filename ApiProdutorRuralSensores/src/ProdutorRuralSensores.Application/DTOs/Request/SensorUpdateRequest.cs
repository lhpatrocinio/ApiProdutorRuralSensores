namespace ProdutorRuralSensores.Application.DTOs.Request;

/// <summary>
/// DTO para atualização de um sensor existente
/// </summary>
public class SensorUpdateRequest
{
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

    /// <summary>
    /// Indica se o sensor está ativo
    /// </summary>
    public bool Ativo { get; set; } = true;
}
