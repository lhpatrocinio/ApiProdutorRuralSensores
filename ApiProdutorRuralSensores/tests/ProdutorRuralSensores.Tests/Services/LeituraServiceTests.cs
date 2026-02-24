using FluentAssertions;
using Moq;
using AutoMapper;
using ProdutorRuralSensores.Application.DTOs.Request;
using ProdutorRuralSensores.Application.DTOs.Response;
using ProdutorRuralSensores.Application.Services;
using ProdutorRuralSensores.Application.Services.Interfaces;
using ProdutorRuralSensores.Domain.Entities;
using ProdutorRuralSensores.Domain.Interfaces;

namespace ProdutorRuralSensores.Tests.Services;

public class LeituraServiceTests
{
    private readonly Mock<ILeituraSensorRepository> _leituraRepoMock;
    private readonly Mock<ISensorRepository> _sensorRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILeituraEventPublisher> _eventPublisherMock;
    private readonly LeituraService _service;

    public LeituraServiceTests()
    {
        _leituraRepoMock = new Mock<ILeituraSensorRepository>();
        _sensorRepoMock = new Mock<ISensorRepository>();
        _mapperMock = new Mock<IMapper>();
        _eventPublisherMock = new Mock<ILeituraEventPublisher>();
        _service = new LeituraService(
            _leituraRepoMock.Object,
            _sensorRepoMock.Object,
            _mapperMock.Object,
            _eventPublisherMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_QuandoExiste_DeveRetornarLeitura()
    {
        var id = Guid.NewGuid();
        var leitura = new LeituraSensor { Id = id, TalhaoId = Guid.NewGuid(), Temperatura = 25m };
        var response = new LeituraResponse { Id = id, Temperatura = 25m };
        _leituraRepoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(leitura);
        _mapperMock.Setup(m => m.Map<LeituraResponse>(leitura)).Returns(response);

        var result = await _service.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Temperatura.Should().Be(25m);
    }

    [Fact]
    public async Task GetByIdAsync_QuandoNaoExiste_DeveRetornarNull()
    {
        _leituraRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((LeituraSensor?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateAsync_ComSensorId_DeveAtualizarUltimaLeitura()
    {
        var sensorId = Guid.NewGuid();
        var talhaoId = Guid.NewGuid();
        var sensor = new Sensor { Id = sensorId, Codigo = "SENS-001", Ativo = true };
        var request = new LeituraCreateRequest
        {
            TalhaoId = talhaoId,
            SensorId = sensorId,
            Temperatura = 30m,
            UmidadeSolo = 60m
        };
        var leitura = new LeituraSensor { Id = Guid.NewGuid(), TalhaoId = talhaoId, SensorId = sensorId };
        var response = new LeituraResponse { Id = leitura.Id, SensorId = sensorId };

        _mapperMock.Setup(m => m.Map<LeituraSensor>(request)).Returns(leitura);
        _sensorRepoMock.Setup(r => r.GetByIdAsync(sensorId)).ReturnsAsync(sensor);
        _sensorRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Sensor>())).Returns(Task.CompletedTask);
        _leituraRepoMock.Setup(r => r.AddAsync(It.IsAny<LeituraSensor>())).ReturnsAsync(leitura);
        _mapperMock.Setup(m => m.Map<LeituraResponse>(It.IsAny<LeituraSensor>())).Returns(response);
        _eventPublisherMock.Setup(p => p.PublishLeituraRecebidaAsync(It.IsAny<LeituraResponse>()))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(request);

        result.Should().NotBeNull();
        _sensorRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Sensor>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_ComCodigoSensor_DeveResolverSensorPorCodigo()
    {
        var talhaoId = Guid.NewGuid();
        var sensorId = Guid.NewGuid();
        var sensor = new Sensor { Id = sensorId, Codigo = "SENS-001" };
        var request = new LeituraCreateRequest
        {
            TalhaoId = talhaoId,
            CodigoSensor = "SENS-001",
            Temperatura = 28m
        };
        var leitura = new LeituraSensor { Id = Guid.NewGuid(), TalhaoId = talhaoId };
        var response = new LeituraResponse { Id = leitura.Id };

        _mapperMock.Setup(m => m.Map<LeituraSensor>(request)).Returns(leitura);
        _sensorRepoMock.Setup(r => r.GetByCodigoAsync("SENS-001")).ReturnsAsync(sensor);
        _sensorRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Sensor>())).Returns(Task.CompletedTask);
        _sensorRepoMock.Setup(r => r.GetByIdAsync(sensorId)).ReturnsAsync(sensor);
        _leituraRepoMock.Setup(r => r.AddAsync(It.IsAny<LeituraSensor>())).ReturnsAsync(leitura);
        _mapperMock.Setup(m => m.Map<LeituraResponse>(It.IsAny<LeituraSensor>())).Returns(response);
        _eventPublisherMock.Setup(p => p.PublishLeituraRecebidaAsync(It.IsAny<LeituraResponse>()))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(request);

        result.Should().NotBeNull();
        _sensorRepoMock.Verify(r => r.GetByCodigoAsync("SENS-001"), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_DevePublicarEvento()
    {
        var request = new LeituraCreateRequest
        {
            TalhaoId = Guid.NewGuid(),
            SensorId = Guid.NewGuid(),
            Temperatura = 25m
        };
        var leitura = new LeituraSensor { Id = Guid.NewGuid(), TalhaoId = request.TalhaoId, SensorId = request.SensorId };
        var sensor = new Sensor { Id = request.SensorId!.Value, Codigo = "S1" };
        var response = new LeituraResponse { Id = leitura.Id, SensorId = request.SensorId };

        _mapperMock.Setup(m => m.Map<LeituraSensor>(request)).Returns(leitura);
        _sensorRepoMock.Setup(r => r.GetByIdAsync(request.SensorId.Value)).ReturnsAsync(sensor);
        _sensorRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Sensor>())).Returns(Task.CompletedTask);
        _leituraRepoMock.Setup(r => r.AddAsync(It.IsAny<LeituraSensor>())).ReturnsAsync(leitura);
        _mapperMock.Setup(m => m.Map<LeituraResponse>(It.IsAny<LeituraSensor>())).Returns(response);
        _eventPublisherMock.Setup(p => p.PublishLeituraRecebidaAsync(It.IsAny<LeituraResponse>()))
            .Returns(Task.CompletedTask);

        await _service.CreateAsync(request);

        _eventPublisherMock.Verify(p => p.PublishLeituraRecebidaAsync(It.IsAny<LeituraResponse>()), Times.Once);
    }

    [Fact]
    public async Task CreateBatchAsync_DeveCriarMultiplasLeituras()
    {
        var request = new LeituraBatchRequest
        {
            Leituras = new List<LeituraCreateRequest>
            {
                new() { TalhaoId = Guid.NewGuid(), SensorId = Guid.NewGuid(), Temperatura = 25m },
                new() { TalhaoId = Guid.NewGuid(), SensorId = Guid.NewGuid(), Temperatura = 26m }
            }
        };

        foreach (var l in request.Leituras)
        {
            var leitura = new LeituraSensor { Id = Guid.NewGuid(), TalhaoId = l.TalhaoId, SensorId = l.SensorId };
            var sensor = new Sensor { Id = l.SensorId!.Value, Codigo = "S" };
            _mapperMock.Setup(m => m.Map<LeituraSensor>(l)).Returns(leitura);
            _sensorRepoMock.Setup(r => r.GetByIdAsync(l.SensorId.Value)).ReturnsAsync(sensor);
            _sensorRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Sensor>())).Returns(Task.CompletedTask);
            _leituraRepoMock.Setup(r => r.AddAsync(It.IsAny<LeituraSensor>())).ReturnsAsync(leitura);
            _mapperMock.Setup(m => m.Map<LeituraResponse>(It.IsAny<LeituraSensor>()))
                .Returns(new LeituraResponse { Id = leitura.Id });
            _eventPublisherMock.Setup(p => p.PublishLeituraRecebidaAsync(It.IsAny<LeituraResponse>()))
                .Returns(Task.CompletedTask);
        }

        var result = await _service.CreateBatchAsync(request);

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetTalhaoStatusAsync_DeveRetornarStatusCompleto()
    {
        var talhaoId = Guid.NewGuid();
        var ultimaLeitura = new LeituraSensor
        {
            TalhaoId = talhaoId,
            UmidadeSolo = 50m,
            Temperatura = 28m,
            Precipitacao = 5m,
            UmidadeAr = 60m,
            DataHoraLeitura = DateTime.UtcNow
        };
        var sensores = new List<Sensor>
        {
            new() { Id = Guid.NewGuid(), TalhaoId = talhaoId, Ativo = true },
            new() { Id = Guid.NewGuid(), TalhaoId = talhaoId, Ativo = false }
        };
        var leituras24h = new List<LeituraSensor>
        {
            new() { Precipitacao = 10m },
            new() { Precipitacao = 5m }
        };

        _leituraRepoMock.Setup(r => r.GetUltimaLeituraAsync(talhaoId)).ReturnsAsync(ultimaLeitura);
        _leituraRepoMock.Setup(r => r.GetMediaUmidadeAsync(talhaoId, 24)).ReturnsAsync(55m);
        _leituraRepoMock.Setup(r => r.GetMediaTemperaturaAsync(talhaoId, 24)).ReturnsAsync(27m);
        _sensorRepoMock.Setup(r => r.GetByTalhaoIdAsync(talhaoId)).ReturnsAsync(sensores);
        _leituraRepoMock.Setup(r => r.GetUltimas24HorasAsync(talhaoId)).ReturnsAsync(leituras24h);

        var result = await _service.GetTalhaoStatusAsync(talhaoId);

        result.TalhaoId.Should().Be(talhaoId);
        result.TotalSensores.Should().Be(2);
        result.SensoresAtivos.Should().Be(1);
        result.UltimaUmidadeSolo.Should().Be(50m);
        result.UltimaTemperatura.Should().Be(28m);
        result.MediaUmidadeSolo24h.Should().Be(55m);
        result.MediaTemperatura24h.Should().Be(27m);
        result.TotalPrecipitacao24h.Should().Be(15m);
    }

    [Fact]
    public async Task GetWithFiltrosAsync_ComTalhaoEPeriodo_DeveFiltrarPorPeriodo()
    {
        var talhaoId = Guid.NewGuid();
        var inicio = DateTime.UtcNow.AddHours(-24);
        var fim = DateTime.UtcNow;
        var leituras = new List<LeituraSensor> { new() { TalhaoId = talhaoId, Temperatura = 25m } };
        var responses = new List<LeituraResponse> { new() { TalhaoId = talhaoId } };
        var filtro = new LeituraFiltroRequest
        {
            TalhaoId = talhaoId,
            DataInicio = inicio,
            DataFim = fim,
            Limite = 100
        };

        _leituraRepoMock.Setup(r => r.GetByPeriodoAsync(talhaoId, inicio, fim)).ReturnsAsync(leituras);
        _mapperMock.Setup(m => m.Map<IEnumerable<LeituraResponse>>(It.IsAny<IEnumerable<LeituraSensor>>()))
            .Returns(responses);

        var result = await _service.GetWithFiltrosAsync(filtro);

        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetWithFiltrosAsync_SemFiltros_DeveRetornarVazio()
    {
        var filtro = new LeituraFiltroRequest();

        var result = await _service.GetWithFiltrosAsync(filtro);

        result.Should().BeEmpty();
    }

    [Fact]
    public void LeituraService_SemEventPublisher_DeveAceitarNulo()
    {
        var service = new LeituraService(
            _leituraRepoMock.Object,
            _sensorRepoMock.Object,
            _mapperMock.Object,
            null);

        service.Should().NotBeNull();
    }
}
