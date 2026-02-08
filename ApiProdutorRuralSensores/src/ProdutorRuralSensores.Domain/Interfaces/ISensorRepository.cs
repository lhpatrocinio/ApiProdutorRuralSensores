using ProdutorRuralSensores.Domain.Entities;

namespace ProdutorRuralSensores.Domain.Interfaces;

/// <summary>
/// Interface de repositório para Sensor
/// </summary>
public interface ISensorRepository : IRepository<Sensor>
{
    Task<Sensor?> GetByCodigoAsync(string codigo);
    Task<IEnumerable<Sensor>> GetByTalhaoIdAsync(Guid talhaoId);
    Task<IEnumerable<Sensor>> GetAtivosAsync();
    Task<Sensor?> GetWithLeiturasAsync(Guid id, int ultimasLeituras = 10);
}
