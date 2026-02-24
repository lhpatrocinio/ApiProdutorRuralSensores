namespace ProdutorRuralSensores.Application.DTOs.Response;

/// <summary>
/// DTO de resposta para leitura agregada (por hora ou dia)
/// </summary>
public class LeituraAgregadaResponse
{
    public Guid TalhaoId { get; set; }
    public DateTime Periodo { get; set; }
    public string TipoAgregacao { get; set; } = string.Empty; // "hora" ou "dia"
    public int QuantidadeLeituras { get; set; }

    public decimal? UmidadeSoloMedia { get; set; }
    public decimal? UmidadeSoloMinima { get; set; }
    public decimal? UmidadeSoloMaxima { get; set; }

    public decimal? TemperaturaMedia { get; set; }
    public decimal? TemperaturaMinima { get; set; }
    public decimal? TemperaturaMaxima { get; set; }

    public decimal? PrecipitacaoTotal { get; set; }

    public decimal? UmidadeArMedia { get; set; }
    public decimal? VelocidadeVentoMedia { get; set; }
    public decimal? RadiacaoSolarMedia { get; set; }
    public decimal? PressaoAtmosfericaMedia { get; set; }
}
