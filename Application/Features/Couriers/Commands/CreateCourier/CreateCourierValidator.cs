using FluentValidation;

namespace Application.Features.Couriers.Commands.CreateCourier;

public class CreateCourierValidator : AbstractValidator<CreateCourierCommand>
{
    public CreateCourierValidator()
    {
        RuleFor(x => x.Dto.Identificador)
            .NotEmpty()
            .WithMessage("Identificador é obrigatório");

        RuleFor(x => x.Dto.Nome)
            .NotEmpty()
            .WithMessage("Nome é obrigatório")
            .MaximumLength(200)
            .WithMessage("Nome não pode exceder 200 caracteres");

        RuleFor(x => x.Dto.Cnpj)
            .NotEmpty()
            .WithMessage("CNPJ é obrigatório")
            .Length(14)
            .WithMessage("CNPJ deve ter 14 caracteres");

        RuleFor(x => x.Dto.DataNascimento)
            .NotEmpty()
            .WithMessage("Data de nascimento é obrigatória")
            .LessThan(DateTime.Today)
            .WithMessage("Data de nascimento deve estar no passado");

        RuleFor(x => x.Dto.NumeroCnh)
            .NotEmpty()
            .WithMessage("Número da CNH é obrigatório")
            .MaximumLength(20)
            .WithMessage("Número da CNH não pode exceder 20 caracteres");

        RuleFor(x => x.Dto.TipoCnh)
            .NotEmpty()
            .WithMessage("Tipo da CNH é obrigatório");
    }
}
