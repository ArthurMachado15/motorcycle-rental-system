using Application.DTOs;
using MediatR;

namespace Application.Features.Motorcycles.Queries.GetMotorcycleByIdentifier;

public record GetMotorcycleByIdentifierQuery(string Identifier) : IRequest<MotorcycleDto>;
