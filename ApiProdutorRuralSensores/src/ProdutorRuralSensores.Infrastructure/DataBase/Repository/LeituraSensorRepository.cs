using Microsoft.EntityFrameworkCore;
using ProdutorRuralSensores.Domain.Entities;
using ProdutorRuralSensores.Domain.Interfaces;
using ProdutorRuralSensores.Infrastructure.DataBase.EntityFramework.Context;

namespace ProdutorRuralSensores.Infrastructure.DataBase.Repository;

/// <summary>
/// Implementação do repositório de LeituraSensor
/// </summary>
public class LeituraSensorRepository : ILeituraSensorRepository
{
    private readonly ApplicationDbContext _context;

    public LeituraSensorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<LeituraSensor?> GetByIdAsync(Guid id)
    {
        return await _context.LeiturasSensores
            .AsNoTracking()
            .Include(l => l.Sensor)
            .FirstOrDefaultAsync(l => l.Id == id);
    }

    public async Task<LeituraSensor> AddAsync(LeituraSensor leitura)
    {
        await _context.LeiturasSensores.AddAsync(leitura);
        await _context.SaveChangesAsync();
        return leitura;
    }

    public async Task AddRangeAsync(IEnumerable<LeituraSensor> leituras)
    {
        await _context.LeiturasSensores.AddRangeAsync(leituras);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<LeituraSensor>> GetByTalhaoIdAsync(Guid talhaoId, int limite = 100)
    {
        return await _context.LeiturasSensores
            .AsNoTracking()
            .Include(l => l.Sensor)
            .Where(l => l.TalhaoId == talhaoId)
            .OrderByDescending(l => l.DataHoraLeitura)
            .Take(limite)
            .ToListAsync();
    }

    public async Task<IEnumerable<LeituraSensor>> GetBySensorIdAsync(Guid sensorId, int limite = 100)
    {
        return await _context.LeiturasSensores
            .AsNoTracking()
            .Include(l => l.Sensor)
            .Where(l => l.SensorId == sensorId)
            .OrderByDescending(l => l.DataHoraLeitura)
            .Take(limite)
            .ToListAsync();
    }

    public async Task<IEnumerable<LeituraSensor>> GetByPeriodoAsync(Guid talhaoId, DateTime inicio, DateTime fim)
    {
        return await _context.LeiturasSensores
            .AsNoTracking()
            .Include(l => l.Sensor)
            .Where(l => l.TalhaoId == talhaoId && 
                        l.DataHoraLeitura >= inicio && 
                        l.DataHoraLeitura <= fim)
            .OrderByDescending(l => l.DataHoraLeitura)
            .ToListAsync();
    }

    public async Task<LeituraSensor?> GetUltimaLeituraAsync(Guid talhaoId)
    {
        return await _context.LeiturasSensores
            .AsNoTracking()
            .Include(l => l.Sensor)
            .Where(l => l.TalhaoId == talhaoId)
            .OrderByDescending(l => l.DataHoraLeitura)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<LeituraSensor>> GetUltimas24HorasAsync(Guid talhaoId)
    {
        var dataLimite = DateTime.UtcNow.AddHours(-24);
        return await _context.LeiturasSensores
            .AsNoTracking()
            .Include(l => l.Sensor)
            .Where(l => l.TalhaoId == talhaoId && l.DataHoraLeitura >= dataLimite)
            .OrderByDescending(l => l.DataHoraLeitura)
            .ToListAsync();
    }

    public async Task<decimal?> GetMediaUmidadeAsync(Guid talhaoId, int horas = 24)
    {
        var dataLimite = DateTime.UtcNow.AddHours(-horas);
        var leituras = await _context.LeiturasSensores
            .AsNoTracking()
            .Where(l => l.TalhaoId == talhaoId && 
                        l.DataHoraLeitura >= dataLimite &&
                        l.UmidadeSolo.HasValue)
            .Select(l => l.UmidadeSolo)
            .ToListAsync();

        return leituras.Any() ? leituras.Average() : null;
    }

    public async Task<decimal?> GetMediaTemperaturaAsync(Guid talhaoId, int horas = 24)
    {
        var dataLimite = DateTime.UtcNow.AddHours(-horas);
        var leituras = await _context.LeiturasSensores
            .AsNoTracking()
            .Where(l => l.TalhaoId == talhaoId && 
                        l.DataHoraLeitura >= dataLimite &&
                        l.Temperatura.HasValue)
            .Select(l => l.Temperatura)
            .ToListAsync();

        return leituras.Any() ? leituras.Average() : null;
    }
}
