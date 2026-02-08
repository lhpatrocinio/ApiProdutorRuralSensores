namespace ProdutorRuralSensores.Application.DTOs.Response;

/// <summary>
/// DTO de resposta para sensor
/// </summary>
public class SensorResponse
{
    public Guid Id { get; set; }
    public Guid TalhaoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string? Modelo { get; set; }
    public string? Fabricante { get; set; }
    public DateTime? DataInstalacao { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool Ativo { get; set; }
    public DateTime? UltimaLeitura { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
