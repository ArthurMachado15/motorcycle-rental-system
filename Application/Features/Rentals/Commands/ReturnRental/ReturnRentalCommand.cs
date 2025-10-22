using Application.DTOs;
using MediatR;

namespace Application.Features.Rentals.Commands.ReturnRental;

public record ReturnRentalCommand(Guid RentalId, DateOnly ReturnDate) : IRequest<RentalDto>;
