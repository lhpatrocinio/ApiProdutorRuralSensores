namespace ProdutorRuralSensores.Application.DTOs.Request;

/// <summary>
/// DTO para filtros de consulta de leituras
/// </summary>
public class LeituraFiltroRequest
{
    /// <summary>
    /// ID do talhão para filtrar
    /// </summary>
    public Guid? TalhaoId { get; set; }

    /// <summary>
    /// ID do sensor para filtrar
    /// </summary>
    public Guid? SensorId { get; set; }

    /// <summary>
    /// Data inicial do período
    /// </summary>
    public DateTime? DataInicio { get; set; }

    /// <summary>
    /// Data final do período
    /// </summary>
    public DateTime? DataFim { get; set; }

    /// <summary>
    /// Limite de registros a retornar (default: 100)
    /// </summary>
    public int Limite { get; set; } = 100;

    /// <summary>
    /// Tipo de agregação (nenhum, hora, dia)
    /// </summary>
    public string? Agregacao { get; set; }
}
