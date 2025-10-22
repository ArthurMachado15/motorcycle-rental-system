using Application.DTOs;
using MediatR;

namespace Application.Features.Rentals.Queries.GetRentalById;

public record GetRentalByIdQuery(Guid Id) : IRequest<RentalDto>;
