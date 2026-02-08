using FluentValidation;
using ProdutorRuralSensores.Application.DTOs.Request;

namespace ProdutorRuralSensores.Application.Validators;

/// <summary>
/// Validador para criação de leituras em batch
/// </summary>
public class LeituraBatchRequestValidator : AbstractValidator<LeituraBatchRequest>
{
    public LeituraBatchRequestValidator()
    {
        RuleFor(x => x.Leituras)
            .NotEmpty()
            .WithMessage("É necessário informar pelo menos uma leitura")
            .Must(x => x.Count <= 1000)
            .WithMessage("O máximo de leituras por batch é 1000");

        RuleForEach(x => x.Leituras)
            .SetValidator(new LeituraCreateRequestValidator());
    }
}
