using AutoMapper;
using ProdutorRuralSensores.Application.DTOs.Request;
using ProdutorRuralSensores.Application.DTOs.Response;
using ProdutorRuralSensores.Application.Services.Interfaces;
using ProdutorRuralSensores.Domain.Entities;
using ProdutorRuralSensores.Domain.Interfaces;

namespace ProdutorRuralSensores.Application.Services;

/// <summary>
/// Serviço para gerenciamento de sensores
/// </summary>
public class SensorService : ISensorService
{
    private readonly ISensorRepository _sensorRepository;
    private readonly IMapper _mapper;

    public SensorService(ISensorRepository sensorRepository, IMapper mapper)
    {
        _sensorRepository = sensorRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<SensorResponse>> GetAllAsync()
    {
        var sensores = await _sensorRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<SensorResponse>>(sensores);
    }

    public async Task<SensorResponse?> GetByIdAsync(Guid id)
    {
        var sensor = await _sensorRepository.GetByIdAsync(id);
        return sensor == null ? null : _mapper.Map<SensorResponse>(sensor);
    }

    public async Task<SensorResponse?> GetByCodigoAsync(string codigo)
    {
        var sensor = await _sensorRepository.GetByCodigoAsync(codigo);
        return sensor == null ? null : _mapper.Map<SensorResponse>(sensor);
    }

    public async Task<IEnumerable<SensorResponse>> GetByTalhaoIdAsync(Guid talhaoId)
    {
        var sensores = await _sensorRepository.GetByTalhaoIdAsync(talhaoId);
        return _mapper.Map<IEnumerable<SensorResponse>>(sensores);
    }

    public async Task<IEnumerable<SensorResponse>> GetAtivosAsync()
    {
        var sensores = await _sensorRepository.GetAtivosAsync();
        return _mapper.Map<IEnumerable<SensorResponse>>(sensores);
    }

    public async Task<SensorComLeiturasResponse?> GetWithLeiturasAsync(Guid id, int ultimasLeituras = 10)
    {
        var sensor = await _sensorRepository.GetWithLeiturasAsync(id, ultimasLeituras);
        return sensor == null ? null : _mapper.Map<SensorComLeiturasResponse>(sensor);
    }

    public async Task<SensorResponse> CreateAsync(SensorCreateRequest request)
    {
        var sensor = _mapper.Map<Sensor>(request);
        sensor.Id = Guid.NewGuid();
        sensor.CreatedAt = DateTime.UtcNow;
        sensor.Ativo = true;

        var created = await _sensorRepository.AddAsync(sensor);
        return _mapper.Map<SensorResponse>(created);
    }

    public async Task<SensorResponse?> UpdateAsync(Guid id, SensorUpdateRequest request)
    {
        var sensor = await _sensorRepository.GetByIdAsync(id);
        if (sensor == null)
            return null;

        sensor.Codigo = request.Codigo;
        sensor.Tipo = request.Tipo;
        sensor.Modelo = request.Modelo;
        sensor.Fabricante = request.Fabricante;
        sensor.DataInstalacao = request.DataInstalacao;
        sensor.Latitude = request.Latitude;
        sensor.Longitude = request.Longitude;
        sensor.Ativo = request.Ativo;
        sensor.UpdatedAt = DateTime.UtcNow;

        await _sensorRepository.UpdateAsync(sensor);
        return _mapper.Map<SensorResponse>(sensor);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var sensor = await _sensorRepository.GetByIdAsync(id);
        if (sensor == null)
            return false;

        await _sensorRepository.DeleteAsync(sensor);
        return true;
    }

    public async Task<bool> AtivarDesativarAsync(Guid id, bool ativo)
    {
        var sensor = await _sensorRepository.GetByIdAsync(id);
        if (sensor == null)
            return false;

        sensor.Ativo = ativo;
        sensor.UpdatedAt = DateTime.UtcNow;
        await _sensorRepository.UpdateAsync(sensor);
        return true;
    }
}
