namespace ProdutorRuralSensores.Domain.Entities;

/// <summary>
/// Entidade que representa uma leitura de sensor IoT
/// </summary>
public class LeituraSensor
{
    public Guid Id { get; set; }
    public Guid TalhaoId { get; set; }
    public Guid? SensorId { get; set; }
    public decimal? UmidadeSolo { get; set; }       // Percentual 0-100
    public decimal? Temperatura { get; set; }        // Celsius
    public decimal? Precipitacao { get; set; }       // mm
    public decimal? UmidadeAr { get; set; }          // Percentual 0-100
    public decimal? VelocidadeVento { get; set; }    // km/h
    public string? DirecaoVento { get; set; }        // N, S, E, W, NE, etc.
    public decimal? RadiacaoSolar { get; set; }      // W/m²
    public decimal? PressaoAtmosferica { get; set; } // hPa
    public DateTime DataHoraLeitura { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation
    public virtual Sensor? Sensor { get; set; }
}
