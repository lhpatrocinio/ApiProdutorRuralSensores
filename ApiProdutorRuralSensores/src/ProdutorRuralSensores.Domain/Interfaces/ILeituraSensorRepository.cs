using ProdutorRuralSensores.Domain.Entities;

namespace ProdutorRuralSensores.Domain.Interfaces;

/// <summary>
/// Interface de repositório para LeituraSensor
/// </summary>
public interface ILeituraSensorRepository
{
    Task<LeituraSensor?> GetByIdAsync(Guid id);
    Task<LeituraSensor> AddAsync(LeituraSensor leitura);
    Task AddRangeAsync(IEnumerable<LeituraSensor> leituras);
    Task<IEnumerable<LeituraSensor>> GetByTalhaoIdAsync(Guid talhaoId, int limite = 100);
    Task<IEnumerable<LeituraSensor>> GetBySensorIdAsync(Guid sensorId, int limite = 100);
    Task<IEnumerable<LeituraSensor>> GetByPeriodoAsync(Guid talhaoId, DateTime inicio, DateTime fim);
    Task<LeituraSensor?> GetUltimaLeituraAsync(Guid talhaoId);
    Task<IEnumerable<LeituraSensor>> GetUltimas24HorasAsync(Guid talhaoId);
    Task<decimal?> GetMediaUmidadeAsync(Guid talhaoId, int horas = 24);
    Task<decimal?> GetMediaTemperaturaAsync(Guid talhaoId, int horas = 24);
}
