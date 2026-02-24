using FluentAssertions;
using Moq;
using AutoMapper;
using ProdutorRuralSensores.Application.DTOs.Request;
using ProdutorRuralSensores.Application.DTOs.Response;
using ProdutorRuralSensores.Application.Services;
using ProdutorRuralSensores.Domain.Entities;
using ProdutorRuralSensores.Domain.Interfaces;

namespace ProdutorRuralSensores.Tests.Services;

public class SensorServiceTests
{
    private readonly Mock<ISensorRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly SensorService _service;

    public SensorServiceTests()
    {
        _repoMock = new Mock<ISensorRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new SensorService(_repoMock.Object, _mapperMock.Object);
    }

    private static Sensor CriarSensor(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        TalhaoId = Guid.NewGuid(),
        Codigo = "SENS-001",
        Tipo = TipoSensor.Temperatura,
        Modelo = "DHT22",
        Fabricante = "Test",
        Ativo = true,
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task GetAllAsync_DeveRetornarSensoresMapeados()
    {
        var sensores = new List<Sensor> { CriarSensor(), CriarSensor() };
        var responses = sensores.Select(s => new SensorResponse { Id = s.Id, Codigo = s.Codigo }).ToList();
        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(sensores);
        _mapperMock.Setup(m => m.Map<IEnumerable<SensorResponse>>(sensores)).Returns(responses);

        var result = await _service.GetAllAsync();

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetByIdAsync_QuandoExiste_DeveRetornarSensor()
    {
        var id = Guid.NewGuid();
        var sensor = CriarSensor(id);
        var response = new SensorResponse { Id = id, Codigo = "SENS-001" };
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(sensor);
        _mapperMock.Setup(m => m.Map<SensorResponse>(sensor)).Returns(response);

        var result = await _service.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
    }

    [Fact]
    public async Task GetByIdAsync_QuandoNaoExiste_DeveRetornarNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Sensor?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid());

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByCodigoAsync_QuandoExiste_DeveRetornarSensor()
    {
        var sensor = CriarSensor();
        var response = new SensorResponse { Id = sensor.Id, Codigo = "SENS-001" };
        _repoMock.Setup(r => r.GetByCodigoAsync("SENS-001")).ReturnsAsync(sensor);
        _mapperMock.Setup(m => m.Map<SensorResponse>(sensor)).Returns(response);

        var result = await _service.GetByCodigoAsync("SENS-001");

        result.Should().NotBeNull();
        result!.Codigo.Should().Be("SENS-001");
    }

    [Fact]
    public async Task CreateAsync_DeveCriarSensorComAtivoTrue()
    {
        var request = new SensorCreateRequest
        {
            TalhaoId = Guid.NewGuid(), Codigo = "SENS-002",
            Tipo = TipoSensor.Umidade, Modelo = "SHT30"
        };
        var response = new SensorResponse { Id = Guid.NewGuid(), Codigo = "SENS-002", Ativo = true };
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Sensor>()))
            .ReturnsAsync((Sensor s) => s);
        _mapperMock.Setup(m => m.Map<Sensor>(request)).Returns(new Sensor());
        _mapperMock.Setup(m => m.Map<SensorResponse>(It.IsAny<Sensor>())).Returns(response);

        var result = await _service.CreateAsync(request);

        result.Should().NotBeNull();
        result.Ativo.Should().BeTrue();
        _repoMock.Verify(r => r.AddAsync(It.Is<Sensor>(s => s.Ativo == true)), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_QuandoExiste_DeveAtualizarERetornar()
    {
        var id = Guid.NewGuid();
        var sensor = CriarSensor(id);
        var response = new SensorResponse { Id = id, Codigo = "SENS-UPD" };
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(sensor);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Sensor>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<SensorResponse>(It.IsAny<Sensor>())).Returns(response);

        var request = new SensorUpdateRequest
        {
            Codigo = "SENS-UPD", Tipo = TipoSensor.Multiparametro, Ativo = true
        };
        var result = await _service.UpdateAsync(id, request);

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdateAsync_QuandoNaoExiste_DeveRetornarNull()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Sensor?)null);

        var request = new SensorUpdateRequest { Codigo = "X", Tipo = "Umidade" };
        var result = await _service.UpdateAsync(Guid.NewGuid(), request);

        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_QuandoExiste_DeveRetornarTrue()
    {
        var id = Guid.NewGuid();
        var sensor = CriarSensor(id);
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(sensor);
        _repoMock.Setup(r => r.DeleteAsync(sensor)).Returns(Task.CompletedTask);

        var result = await _service.DeleteAsync(id);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_QuandoNaoExiste_DeveRetornarFalse()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Sensor?)null);

        var result = await _service.DeleteAsync(Guid.NewGuid());

        result.Should().BeFalse();
    }

    [Fact]
    public async Task AtivarDesativarAsync_QuandoExiste_DeveAtualizarERetornarTrue()
    {
        var id = Guid.NewGuid();
        var sensor = CriarSensor(id);
        _repoMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(sensor);
        _repoMock.Setup(r => r.UpdateAsync(It.IsAny<Sensor>())).Returns(Task.CompletedTask);

        var result = await _service.AtivarDesativarAsync(id, false);

        result.Should().BeTrue();
        sensor.Ativo.Should().BeFalse();
    }

    [Fact]
    public async Task AtivarDesativarAsync_QuandoNaoExiste_DeveRetornarFalse()
    {
        _repoMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Sensor?)null);

        var result = await _service.AtivarDesativarAsync(Guid.NewGuid(), true);

        result.Should().BeFalse();
    }

    [Fact]
    public async Task GetAtivosAsync_DeveRetornarApenasSensoresAtivos()
    {
        var sensores = new List<Sensor> { CriarSensor() };
        var responses = new List<SensorResponse> { new() { Id = sensores[0].Id, Ativo = true } };
        _repoMock.Setup(r => r.GetAtivosAsync()).ReturnsAsync(sensores);
        _mapperMock.Setup(m => m.Map<IEnumerable<SensorResponse>>(sensores)).Returns(responses);

        var result = await _service.GetAtivosAsync();

        result.Should().HaveCount(1);
    }
}
