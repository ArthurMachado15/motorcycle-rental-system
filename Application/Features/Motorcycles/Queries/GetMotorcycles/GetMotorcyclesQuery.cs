using Application.DTOs;
using MediatR;

namespace Application.Features.Motorcycles.Queries.GetMotorcycles;

public record GetMotorcyclesQuery(string? LicensePlate = null) : IRequest<IEnumerable<MotorcycleDto>>;
