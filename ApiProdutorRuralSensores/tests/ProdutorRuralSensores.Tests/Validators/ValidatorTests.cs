using FluentAssertions;
using FluentValidation.TestHelper;
using ProdutorRuralSensores.Application.DTOs.Request;
using ProdutorRuralSensores.Application.Validators;
using ProdutorRuralSensores.Domain.Entities;

namespace ProdutorRuralSensores.Tests.Validators;

public class SensorCreateRequestValidatorTests
{
    private readonly SensorCreateRequestValidator _validator = new();

    [Fact]
    public void Validar_RequestValida_NaoDeveTerErros()
    {
        var request = new SensorCreateRequest
        {
            TalhaoId = Guid.NewGuid(),
            Codigo = "SENS-001",
            Tipo = TipoSensor.Temperatura,
            Modelo = "DHT22",
            Latitude = -23.5m,
            Longitude = -46.6m
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_TalhaoIdVazio_DeveTerErro()
    {
        var request = new SensorCreateRequest { TalhaoId = Guid.Empty, Codigo = "S1", Tipo = TipoSensor.Temperatura };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TalhaoId);
    }

    [Fact]
    public void Validar_CodigoVazio_DeveTerErro()
    {
        var request = new SensorCreateRequest { TalhaoId = Guid.NewGuid(), Codigo = "", Tipo = TipoSensor.Temperatura };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Codigo);
    }

    [Fact]
    public void Validar_CodigoMuitoGrande_DeveTerErro()
    {
        var request = new SensorCreateRequest
        {
            TalhaoId = Guid.NewGuid(),
            Codigo = new string('A', 51),
            Tipo = TipoSensor.Temperatura
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Codigo);
    }

    [Fact]
    public void Validar_TipoInvalido_DeveTerErro()
    {
        var request = new SensorCreateRequest
        {
            TalhaoId = Guid.NewGuid(), Codigo = "S1", Tipo = "TipoInvalido"
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Tipo);
    }

    [Theory]
    [InlineData("Umidade")]
    [InlineData("Temperatura")]
    [InlineData("Multiparâmetro")]
    public void Validar_TiposValidos_NaoDeveTerErro(string tipo)
    {
        var request = new SensorCreateRequest
        {
            TalhaoId = Guid.NewGuid(), Codigo = "S1", Tipo = tipo
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveValidationErrorFor(x => x.Tipo);
    }

    [Fact]
    public void Validar_LatitudeForaDaFaixa_DeveTerErro()
    {
        var request = new SensorCreateRequest
        {
            TalhaoId = Guid.NewGuid(), Codigo = "S1",
            Tipo = TipoSensor.Temperatura, Latitude = 91m
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Latitude);
    }

    [Fact]
    public void Validar_LongitudeForaDaFaixa_DeveTerErro()
    {
        var request = new SensorCreateRequest
        {
            TalhaoId = Guid.NewGuid(), Codigo = "S1",
            Tipo = TipoSensor.Temperatura, Longitude = -181m
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Longitude);
    }
}

public class LeituraCreateRequestValidatorTests
{
    private readonly LeituraCreateRequestValidator _validator = new();

    [Fact]
    public void Validar_RequestValida_NaoDeveTerErros()
    {
        var request = new LeituraCreateRequest
        {
            TalhaoId = Guid.NewGuid(),
            SensorId = Guid.NewGuid(),
            Temperatura = 25m,
            UmidadeSolo = 60m
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void Validar_TalhaoIdVazio_DeveTerErro()
    {
        var request = new LeituraCreateRequest
        {
            TalhaoId = Guid.Empty, SensorId = Guid.NewGuid(), Temperatura = 25m
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.TalhaoId);
    }

    [Fact]
    public void Validar_SemSensorIdNemCodigo_DeveTerErro()
    {
        var request = new LeituraCreateRequest
        {
            TalhaoId = Guid.NewGuid(), Temperatura = 25m
        };

        var result = _validator.TestValidate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validar_UmidadeSoloForaDaFaixa_DeveTerErro()
    {
        var request = new LeituraCreateRequest
        {
            TalhaoId = Guid.NewGuid(), SensorId = Guid.NewGuid(),
            UmidadeSolo = 101m
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.UmidadeSolo);
    }

    [Fact]
    public void Validar_TemperaturaForaDaFaixa_DeveTerErro()
    {
        var request = new LeituraCreateRequest
        {
            TalhaoId = Guid.NewGuid(), SensorId = Guid.NewGuid(),
            Temperatura = -51m
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Temperatura);
    }

    [Fact]
    public void Validar_PrecipitacaoNegativa_DeveTerErro()
    {
        var request = new LeituraCreateRequest
        {
            TalhaoId = Guid.NewGuid(), SensorId = Guid.NewGuid(),
            Precipitacao = -1m
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Precipitacao);
    }

    [Fact]
    public void Validar_SemNenhumValorLeitura_DeveTerErro()
    {
        var request = new LeituraCreateRequest
        {
            TalhaoId = Guid.NewGuid(), SensorId = Guid.NewGuid()
        };

        var result = _validator.TestValidate(request);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validar_PressaoAtmosfericaForaDaFaixa_DeveTerErro()
    {
        var request = new LeituraCreateRequest
        {
            TalhaoId = Guid.NewGuid(), SensorId = Guid.NewGuid(),
            PressaoAtmosferica = 700m, Temperatura = 25m
        };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.PressaoAtmosferica);
    }
}

public class LeituraBatchRequestValidatorTests
{
    private readonly LeituraBatchRequestValidator _validator = new();

    [Fact]
    public void Validar_ListaVazia_DeveTerErro()
    {
        var request = new LeituraBatchRequest { Leituras = new List<LeituraCreateRequest>() };

        var result = _validator.TestValidate(request);

        result.ShouldHaveValidationErrorFor(x => x.Leituras);
    }

    [Fact]
    public void Validar_ListaValida_NaoDeveTerErros()
    {
        var request = new LeituraBatchRequest
        {
            Leituras = new List<LeituraCreateRequest>
            {
                new() { TalhaoId = Guid.NewGuid(), SensorId = Guid.NewGuid(), Temperatura = 25m }
            }
        };

        var result = _validator.TestValidate(request);

        result.ShouldNotHaveAnyValidationErrors();
    }
}
