namespace ProdutorRuralSensores.Application.DTOs.Request;

/// <summary>
/// DTO para criação de múltiplas leituras de sensores em batch (dados IoT)
/// </summary>
public class LeituraBatchRequest
{
    /// <summary>
    /// Lista de leituras a serem registradas
    /// </summary>
    public List<LeituraCreateRequest> Leituras { get; set; } = new();
}
