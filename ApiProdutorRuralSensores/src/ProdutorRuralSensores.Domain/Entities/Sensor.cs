namespace ProdutorRuralSensores.Domain.Entities;

/// <summary>
/// Entidade que representa um Sensor IoT instalado no talhão
/// </summary>
public class Sensor : BaseEntity
{
    public Guid TalhaoId { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;  // "Umidade", "Temperatura", "Multiparâmetro"
    public string? Modelo { get; set; }
    public string? Fabricante { get; set; }
    public DateTime? DataInstalacao { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool Ativo { get; set; } = true;
    public DateTime? UltimaLeitura { get; set; }

    // Navigation
    public virtual ICollection<LeituraSensor> Leituras { get; set; } = new List<LeituraSensor>();
}

/// <summary>
/// Tipos de sensores suportados
/// </summary>
public static class TipoSensor
{
    public const string Umidade = "Umidade";
    public const string Temperatura = "Temperatura";
    public const string Multiparametro = "Multiparâmetro";
}
