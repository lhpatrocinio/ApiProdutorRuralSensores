using AutoMapper;
using ProdutorRuralSensores.Application.DTOs.Request;
using ProdutorRuralSensores.Application.DTOs.Response;
using ProdutorRuralSensores.Application.Services.Interfaces;
using ProdutorRuralSensores.Domain.Entities;
using ProdutorRuralSensores.Domain.Interfaces;

namespace ProdutorRuralSensores.Application.Services;

/// <summary>
/// Serviço para gerenciamento de leituras de sensores
/// </summary>
public class LeituraService : ILeituraService
{
    private readonly ILeituraSensorRepository _leituraRepository;
    private readonly ISensorRepository _sensorRepository;
    private readonly IMapper _mapper;
    private readonly ILeituraEventPublisher? _eventPublisher;

    public LeituraService(
        ILeituraSensorRepository leituraRepository,
        ISensorRepository sensorRepository,
        IMapper mapper,
        ILeituraEventPublisher? eventPublisher = null)
    {
        _leituraRepository = leituraRepository;
        _sensorRepository = sensorRepository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    public async Task<LeituraResponse?> GetByIdAsync(Guid id)
    {
        var leitura = await _leituraRepository.GetByIdAsync(id);
        return leitura == null ? null : _mapper.Map<LeituraResponse>(leitura);
    }

    public async Task<IEnumerable<LeituraResponse>> GetByTalhaoIdAsync(Guid talhaoId, int limite = 100)
    {
        var leituras = await _leituraRepository.GetByTalhaoIdAsync(talhaoId, limite);
        return _mapper.Map<IEnumerable<LeituraResponse>>(leituras);
    }

    public async Task<IEnumerable<LeituraResponse>> GetBySensorIdAsync(Guid sensorId, int limite = 100)
    {
        var leituras = await _leituraRepository.GetBySensorIdAsync(sensorId, limite);
        return _mapper.Map<IEnumerable<LeituraResponse>>(leituras);
    }

    public async Task<IEnumerable<LeituraResponse>> GetByPeriodoAsync(Guid talhaoId, DateTime inicio, DateTime fim)
    {
        var leituras = await _leituraRepository.GetByPeriodoAsync(talhaoId, inicio, fim);
        return _mapper.Map<IEnumerable<LeituraResponse>>(leituras);
    }

    public async Task<IEnumerable<LeituraResponse>> GetWithFiltrosAsync(LeituraFiltroRequest filtros)
    {
        IEnumerable<LeituraSensor> leituras;

        if (filtros.TalhaoId.HasValue && filtros.DataInicio.HasValue && filtros.DataFim.HasValue)
        {
            leituras = await _leituraRepository.GetByPeriodoAsync(
                filtros.TalhaoId.Value, 
                filtros.DataInicio.Value, 
                filtros.DataFim.Value);
        }
        else if (filtros.SensorId.HasValue)
        {
            leituras = await _leituraRepository.GetBySensorIdAsync(filtros.SensorId.Value, filtros.Limite);
        }
        else if (filtros.TalhaoId.HasValue)
        {
            leituras = await _leituraRepository.GetByTalhaoIdAsync(filtros.TalhaoId.Value, filtros.Limite);
        }
        else
        {
            // Retorna lista vazia se nenhum filtro foi especificado
            return Enumerable.Empty<LeituraResponse>();
        }

        return _mapper.Map<IEnumerable<LeituraResponse>>(leituras.Take(filtros.Limite));
    }

    public async Task<LeituraResponse?> GetUltimaLeituraAsync(Guid talhaoId)
    {
        var leitura = await _leituraRepository.GetUltimaLeituraAsync(talhaoId);
        return leitura == null ? null : _mapper.Map<LeituraResponse>(leitura);
    }

    public async Task<IEnumerable<LeituraResponse>> GetUltimas24HorasAsync(Guid talhaoId)
    {
        var leituras = await _leituraRepository.GetUltimas24HorasAsync(talhaoId);
        return _mapper.Map<IEnumerable<LeituraResponse>>(leituras);
    }

    public async Task<TalhaoStatusResponse> GetTalhaoStatusAsync(Guid talhaoId)
    {
        var ultimaLeitura = await _leituraRepository.GetUltimaLeituraAsync(talhaoId);
        var mediaUmidade = await _leituraRepository.GetMediaUmidadeAsync(talhaoId, 24);
        var mediaTemperatura = await _leituraRepository.GetMediaTemperaturaAsync(talhaoId, 24);
        var sensores = await _sensorRepository.GetByTalhaoIdAsync(talhaoId);

        var response = new TalhaoStatusResponse
        {
            TalhaoId = talhaoId,
            UltimaLeitura = ultimaLeitura?.DataHoraLeitura,
            TotalSensores = sensores.Count(),
            SensoresAtivos = sensores.Count(s => s.Ativo),
            UltimaUmidadeSolo = ultimaLeitura?.UmidadeSolo,
            UltimaTemperatura = ultimaLeitura?.Temperatura,
            UltimaPrecipitacao = ultimaLeitura?.Precipitacao,
            UltimaUmidadeAr = ultimaLeitura?.UmidadeAr,
            MediaUmidadeSolo24h = mediaUmidade,
            MediaTemperatura24h = mediaTemperatura
        };

        // Calcular precipitação total das últimas 24h
        var leituras24h = await _leituraRepository.GetUltimas24HorasAsync(talhaoId);
        response.TotalPrecipitacao24h = leituras24h
            .Where(l => l.Precipitacao.HasValue)
            .Sum(l => l.Precipitacao ?? 0);

        return response;
    }

    public async Task<LeituraResponse> CreateAsync(LeituraCreateRequest request)
    {
        var leitura = _mapper.Map<LeituraSensor>(request);
        leitura.Id = Guid.NewGuid();
        leitura.DataHoraLeitura = request.DataHoraLeitura ?? DateTime.UtcNow;
        leitura.CreatedAt = DateTime.UtcNow;

        // Se foi informado código do sensor, buscar o ID
        if (!request.SensorId.HasValue && !string.IsNullOrEmpty(request.CodigoSensor))
        {
            var sensor = await _sensorRepository.GetByCodigoAsync(request.CodigoSensor);
            if (sensor != null)
            {
                leitura.SensorId = sensor.Id;
                // Atualizar a última leitura do sensor
                sensor.UltimaLeitura = leitura.DataHoraLeitura;
                await _sensorRepository.UpdateAsync(sensor);
            }
        }
        else if (request.SensorId.HasValue)
        {
            var sensor = await _sensorRepository.GetByIdAsync(request.SensorId.Value);
            if (sensor != null)
            {
                sensor.UltimaLeitura = leitura.DataHoraLeitura;
                await _sensorRepository.UpdateAsync(sensor);
            }
        }

        var created = await _leituraRepository.AddAsync(leitura);
        
        var response = _mapper.Map<LeituraResponse>(created);
        
        // Buscar código do sensor se disponível
        if (created.SensorId.HasValue)
        {
            var sensor = await _sensorRepository.GetByIdAsync(created.SensorId.Value);
            response.CodigoSensor = sensor?.Codigo;
        }

        // Publicar evento para processamento de alertas (não bloqueia se falhar)
        if (_eventPublisher != null)
        {
            await _eventPublisher.PublishLeituraRecebidaAsync(response);
        }

        return response;
    }

    public async Task<IEnumerable<LeituraResponse>> CreateBatchAsync(LeituraBatchRequest request)
    {
        var responses = new List<LeituraResponse>();
        
        foreach (var leituraRequest in request.Leituras)
        {
            var response = await CreateAsync(leituraRequest);
            responses.Add(response);
        }

        return responses;
    }

    public async Task<IEnumerable<LeituraAgregadaResponse>> GetAgregadoAsync(
        Guid talhaoId, 
        DateTime inicio, 
        DateTime fim, 
        string agregacao)
    {
        var leituras = await _leituraRepository.GetByPeriodoAsync(talhaoId, inicio, fim);
        var listaLeituras = leituras.ToList();

        if (!listaLeituras.Any())
            return Enumerable.Empty<LeituraAgregadaResponse>();

        IEnumerable<IGrouping<DateTime, LeituraSensor>> grupos;

        if (agregacao.ToLower() == "hora")
        {
            grupos = listaLeituras.GroupBy(l => new DateTime(
                l.DataHoraLeitura.Year, 
                l.DataHoraLeitura.Month, 
                l.DataHoraLeitura.Day, 
                l.DataHoraLeitura.Hour, 0, 0));
        }
        else // dia
        {
            grupos = listaLeituras.GroupBy(l => l.DataHoraLeitura.Date);
        }

        return grupos.Select(g => new LeituraAgregadaResponse
        {
            TalhaoId = talhaoId,
            Periodo = g.Key,
            TipoAgregacao = agregacao.ToLower(),
            QuantidadeLeituras = g.Count(),
            UmidadeSoloMedia = g.Where(l => l.UmidadeSolo.HasValue).Average(l => l.UmidadeSolo),
            UmidadeSoloMinima = g.Where(l => l.UmidadeSolo.HasValue).Min(l => l.UmidadeSolo),
            UmidadeSoloMaxima = g.Where(l => l.UmidadeSolo.HasValue).Max(l => l.UmidadeSolo),
            TemperaturaMedia = g.Where(l => l.Temperatura.HasValue).Average(l => l.Temperatura),
            TemperaturaMinima = g.Where(l => l.Temperatura.HasValue).Min(l => l.Temperatura),
            TemperaturaMaxima = g.Where(l => l.Temperatura.HasValue).Max(l => l.Temperatura),
            PrecipitacaoTotal = g.Where(l => l.Precipitacao.HasValue).Sum(l => l.Precipitacao),
            UmidadeArMedia = g.Where(l => l.UmidadeAr.HasValue).Average(l => l.UmidadeAr),
            VelocidadeVentoMedia = g.Where(l => l.VelocidadeVento.HasValue).Average(l => l.VelocidadeVento),
            RadiacaoSolarMedia = g.Where(l => l.RadiacaoSolar.HasValue).Average(l => l.RadiacaoSolar),
            PressaoAtmosfericaMedia = g.Where(l => l.PressaoAtmosferica.HasValue).Average(l => l.PressaoAtmosferica)
        }).OrderBy(a => a.Periodo);
    }
}
