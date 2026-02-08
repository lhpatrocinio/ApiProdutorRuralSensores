namespace ProdutorRuralSensores.Application.DTOs.Response;

/// <summary>
/// DTO de resposta resumida do status de um talhão
/// </summary>
public class TalhaoStatusResponse
{
    public Guid TalhaoId { get; set; }
    public DateTime? UltimaLeitura { get; set; }
    public int TotalSensores { get; set; }
    public int SensoresAtivos { get; set; }
    
    /// <summary>
    /// Última leitura de cada tipo de dado
    /// </summary>
    public decimal? UltimaUmidadeSolo { get; set; }
    public decimal? UltimaTemperatura { get; set; }
    public decimal? UltimaPrecipitacao { get; set; }
    public decimal? UltimaUmidadeAr { get; set; }

    /// <summary>
    /// Médias das últimas 24 horas
    /// </summary>
    public decimal? MediaUmidadeSolo24h { get; set; }
    public decimal? MediaTemperatura24h { get; set; }
    public decimal? TotalPrecipitacao24h { get; set; }
}
