using Application.DTOs;
using MediatR;

namespace Application.Features.Rentals.Commands.CreateRental;

public record CreateRentalCommand(CreateRentalDto Dto) : IRequest<RentalDto>;
