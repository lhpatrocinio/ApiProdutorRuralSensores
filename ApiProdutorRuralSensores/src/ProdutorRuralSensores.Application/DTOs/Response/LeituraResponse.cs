namespace ProdutorRuralSensores.Application.DTOs.Response;

/// <summary>
/// DTO de resposta para leitura de sensor
/// </summary>
public class LeituraResponse
{
    public Guid Id { get; set; }
    public Guid TalhaoId { get; set; }
    public Guid? SensorId { get; set; }
    public string? CodigoSensor { get; set; }
    public decimal? UmidadeSolo { get; set; }
    public decimal? Temperatura { get; set; }
    public decimal? Precipitacao { get; set; }
    public decimal? UmidadeAr { get; set; }
    public decimal? VelocidadeVento { get; set; }
    public string? DirecaoVento { get; set; }
    public decimal? RadiacaoSolar { get; set; }
    public decimal? PressaoAtmosferica { get; set; }
    public DateTime DataHoraLeitura { get; set; }
    public DateTime CreatedAt { get; set; }
}
