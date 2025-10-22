using FluentValidation;

namespace Application.Features.Motorcycles.Commands.CreateMotorcycle;

public class CreateMotorcycleValidator : AbstractValidator<CreateMotorcycleCommand>
{
    public CreateMotorcycleValidator()
    {
        RuleFor(x => x.Dto.Identificador)
            .NotEmpty()
            .WithMessage("Identificador é obrigatório");

        RuleFor(x => x.Dto.Ano)
            .GreaterThan(1900)
            .WithMessage("Ano deve ser maior que 1900");

        RuleFor(x => x.Dto.Modelo)
            .NotEmpty()
            .WithMessage("Modelo é obrigatório")
            .MaximumLength(100)
            .WithMessage("Modelo não pode exceder 100 caracteres");

        RuleFor(x => x.Dto.Placa)
            .NotEmpty()
            .WithMessage("Placa é obrigatória")
            .MaximumLength(20)
            .WithMessage("Placa não pode exceder 20 caracteres");
    }
}
