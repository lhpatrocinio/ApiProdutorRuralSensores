using ProdutorRuralSensores.Application.DTOs.Request;
using ProdutorRuralSensores.Application.DTOs.Response;

namespace ProdutorRuralSensores.Application.Services.Interfaces;

/// <summary>
/// Interface de serviço para gerenciamento de sensores
/// </summary>
public interface ISensorService
{
    Task<IEnumerable<SensorResponse>> GetAllAsync();
    Task<SensorResponse?> GetByIdAsync(Guid id);
    Task<SensorResponse?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<SensorResponse>> GetByTalhaoIdAsync(Guid talhaoId);
    Task<IEnumerable<SensorResponse>> GetAtivosAsync();
    Task<SensorComLeiturasResponse?> GetWithLeiturasAsync(Guid id, int ultimasLeituras = 10);
    Task<SensorResponse> CreateAsync(SensorCreateRequest request);
    Task<SensorResponse?> UpdateAsync(Guid id, SensorUpdateRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> AtivarDesativarAsync(Guid id, bool ativo);
}
