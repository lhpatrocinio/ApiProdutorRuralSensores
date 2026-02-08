using ProdutorRuralSensores.Application.DTOs.Request;
using ProdutorRuralSensores.Application.DTOs.Response;

namespace ProdutorRuralSensores.Application.Services.Interfaces;

/// <summary>
/// Interface de serviço para gerenciamento de leituras de sensores
/// </summary>
public interface ILeituraService
{
    Task<LeituraResponse?> GetByIdAsync(Guid id);
    Task<IEnumerable<LeituraResponse>> GetByTalhaoIdAsync(Guid talhaoId, int limite = 100);
    Task<IEnumerable<LeituraResponse>> GetBySensorIdAsync(Guid sensorId, int limite = 100);
    Task<IEnumerable<LeituraResponse>> GetByPeriodoAsync(Guid talhaoId, DateTime inicio, DateTime fim);
    Task<IEnumerable<LeituraResponse>> GetWithFiltrosAsync(LeituraFiltroRequest filtros);
    Task<LeituraResponse?> GetUltimaLeituraAsync(Guid talhaoId);
    Task<IEnumerable<LeituraResponse>> GetUltimas24HorasAsync(Guid talhaoId);
    Task<TalhaoStatusResponse> GetTalhaoStatusAsync(Guid talhaoId);
    Task<LeituraResponse> CreateAsync(LeituraCreateRequest request);
    Task<IEnumerable<LeituraResponse>> CreateBatchAsync(LeituraBatchRequest request);
    Task<IEnumerable<LeituraAgregadaResponse>> GetAgregadoAsync(Guid talhaoId, DateTime inicio, DateTime fim, string agregacao);
}
