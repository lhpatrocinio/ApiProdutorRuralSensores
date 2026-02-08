using FluentValidation;
using ProdutorRuralSensores.Application.DTOs.Request;
using ProdutorRuralSensores.Domain.Entities;

namespace ProdutorRuralSensores.Application.Validators;

/// <summary>
/// Validador para criação de sensor
/// </summary>
public class SensorCreateRequestValidator : AbstractValidator<SensorCreateRequest>
{
    public SensorCreateRequestValidator()
    {
        RuleFor(x => x.TalhaoId)
            .NotEmpty()
            .WithMessage("O ID do talhão é obrigatório");

        RuleFor(x => x.Codigo)
            .NotEmpty()
            .WithMessage("O código do sensor é obrigatório")
            .MaximumLength(50)
            .WithMessage("O código do sensor deve ter no máximo 50 caracteres");

        RuleFor(x => x.Tipo)
            .NotEmpty()
            .WithMessage("O tipo do sensor é obrigatório")
            .Must(tipo => tipo == TipoSensor.Umidade || 
                          tipo == TipoSensor.Temperatura || 
                          tipo == TipoSensor.Multiparametro)
            .WithMessage($"O tipo do sensor deve ser: {TipoSensor.Umidade}, {TipoSensor.Temperatura} ou {TipoSensor.Multiparametro}");

        RuleFor(x => x.Modelo)
            .MaximumLength(100)
            .WithMessage("O modelo deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Modelo));

        RuleFor(x => x.Fabricante)
            .MaximumLength(100)
            .WithMessage("O fabricante deve ter no máximo 100 caracteres")
            .When(x => !string.IsNullOrEmpty(x.Fabricante));

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("A latitude deve estar entre -90 e 90")
            .When(x => x.Latitude.HasValue);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("A longitude deve estar entre -180 e 180")
            .When(x => x.Longitude.HasValue);
    }
}
