using FluentValidation;
using ProdutorRuralSensores.Application.DTOs.Request;

namespace ProdutorRuralSensores.Application.Validators;

/// <summary>
/// Validador para criação de leitura de sensor
/// </summary>
public class LeituraCreateRequestValidator : AbstractValidator<LeituraCreateRequest>
{
    public LeituraCreateRequestValidator()
    {
        RuleFor(x => x.TalhaoId)
            .NotEmpty()
            .WithMessage("O ID do talhão é obrigatório");

        // Deve ter pelo menos SensorId ou CodigoSensor
        RuleFor(x => x)
            .Must(x => x.SensorId.HasValue || !string.IsNullOrEmpty(x.CodigoSensor))
            .WithMessage("É necessário informar o SensorId ou o CodigoSensor");

        RuleFor(x => x.CodigoSensor)
            .MaximumLength(50)
            .WithMessage("O código do sensor deve ter no máximo 50 caracteres")
            .When(x => !string.IsNullOrEmpty(x.CodigoSensor));

        // Validações de range para valores de leitura
        RuleFor(x => x.UmidadeSolo)
            .InclusiveBetween(0, 100)
            .WithMessage("A umidade do solo deve estar entre 0 e 100%")
            .When(x => x.UmidadeSolo.HasValue);

        RuleFor(x => x.UmidadeAr)
            .InclusiveBetween(0, 100)
            .WithMessage("A umidade do ar deve estar entre 0 e 100%")
            .When(x => x.UmidadeAr.HasValue);

        RuleFor(x => x.Temperatura)
            .InclusiveBetween(-50, 60)
            .WithMessage("A temperatura deve estar entre -50°C e 60°C")
            .When(x => x.Temperatura.HasValue);

        RuleFor(x => x.Precipitacao)
            .GreaterThanOrEqualTo(0)
            .WithMessage("A precipitação não pode ser negativa")
            .When(x => x.Precipitacao.HasValue);

        RuleFor(x => x.VelocidadeVento)
            .GreaterThanOrEqualTo(0)
            .WithMessage("A velocidade do vento não pode ser negativa")
            .LessThanOrEqualTo(500)
            .WithMessage("A velocidade do vento não pode ser maior que 500 km/h")
            .When(x => x.VelocidadeVento.HasValue);

        RuleFor(x => x.DirecaoVento)
            .MaximumLength(10)
            .WithMessage("A direção do vento deve ter no máximo 10 caracteres")
            .When(x => !string.IsNullOrEmpty(x.DirecaoVento));

        RuleFor(x => x.RadiacaoSolar)
            .GreaterThanOrEqualTo(0)
            .WithMessage("A radiação solar não pode ser negativa")
            .When(x => x.RadiacaoSolar.HasValue);

        RuleFor(x => x.PressaoAtmosferica)
            .InclusiveBetween(800, 1100)
            .WithMessage("A pressão atmosférica deve estar entre 800 e 1100 hPa")
            .When(x => x.PressaoAtmosferica.HasValue);

        RuleFor(x => x.DataHoraLeitura)
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
            .WithMessage("A data/hora da leitura não pode ser no futuro")
            .When(x => x.DataHoraLeitura.HasValue);

        // Deve ter pelo menos um valor de leitura
        RuleFor(x => x)
            .Must(x => x.UmidadeSolo.HasValue || 
                       x.Temperatura.HasValue || 
                       x.Precipitacao.HasValue || 
                       x.UmidadeAr.HasValue ||
                       x.VelocidadeVento.HasValue ||
                       x.RadiacaoSolar.HasValue ||
                       x.PressaoAtmosferica.HasValue)
            .WithMessage("É necessário informar pelo menos um valor de leitura");
    }
}
