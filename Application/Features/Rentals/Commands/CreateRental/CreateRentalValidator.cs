using FluentValidation;

namespace Application.Features.Rentals.Commands.CreateRental;

public class CreateRentalValidator : AbstractValidator<CreateRentalCommand>
{
    public CreateRentalValidator()
    {
        RuleFor(x => x.Dto.EntregadorId)
            .NotEmpty()
            .WithMessage("ID do entregador é obrigatório");

        RuleFor(x => x.Dto.MotoId)
            .NotEmpty()
            .WithMessage("ID da moto é obrigatório");

        RuleFor(x => x.Dto.Plano)
            .Must(plano => new[] { 7, 15, 30, 45, 50 }.Contains(plano))
            .WithMessage("Plano deve ser 7, 15, 30, 45 ou 50 dias");
    }
}
