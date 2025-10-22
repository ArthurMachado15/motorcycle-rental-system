using Application.DTOs;
using MediatR;

namespace Application.Features.Motorcycles.Queries.GetMotorcycleById;

public record GetMotorcycleByIdQuery(Guid Id) : IRequest<MotorcycleDto>;
