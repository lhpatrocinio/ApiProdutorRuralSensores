using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ProdutorRuralSensores.Domain.Entities;
using ProdutorRuralSensores.Domain.Interfaces;
using ProdutorRuralSensores.Infrastructure.DataBase.EntityFramework.Context;

namespace ProdutorRuralSensores.Infrastructure.DataBase.Repository;

/// <summary>
/// Implementação do repositório de Sensor
/// </summary>
public class SensorRepository : ISensorRepository
{
    private readonly ApplicationDbContext _context;

    public SensorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Sensor?> GetByIdAsync(Guid id)
    {
        return await _context.Sensores
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<IEnumerable<Sensor>> GetAllAsync()
    {
        return await _context.Sensores
            .AsNoTracking()
            .OrderBy(s => s.Codigo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sensor>> FindAsync(Expression<Func<Sensor, bool>> predicate)
    {
        return await _context.Sensores
            .AsNoTracking()
            .Where(predicate)
            .ToListAsync();
    }

    public async Task<Sensor> AddAsync(Sensor entity)
    {
        await _context.Sensores.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(Sensor entity)
    {
        _context.Sensores.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Sensor entity)
    {
        _context.Sensores.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.Sensores.AnyAsync(s => s.Id == id);
    }

    public async Task<Sensor?> GetByCodigoAsync(string codigo)
    {
        return await _context.Sensores
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Codigo == codigo);
    }

    public async Task<IEnumerable<Sensor>> GetByTalhaoIdAsync(Guid talhaoId)
    {
        return await _context.Sensores
            .AsNoTracking()
            .Where(s => s.TalhaoId == talhaoId)
            .OrderBy(s => s.Codigo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sensor>> GetAtivosAsync()
    {
        return await _context.Sensores
            .AsNoTracking()
            .Where(s => s.Ativo)
            .OrderBy(s => s.Codigo)
            .ToListAsync();
    }

    public async Task<Sensor?> GetWithLeiturasAsync(Guid id, int ultimasLeituras = 10)
    {
        return await _context.Sensores
            .AsNoTracking()
            .Include(s => s.Leituras
                .OrderByDescending(l => l.DataHoraLeitura)
                .Take(ultimasLeituras))
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}
