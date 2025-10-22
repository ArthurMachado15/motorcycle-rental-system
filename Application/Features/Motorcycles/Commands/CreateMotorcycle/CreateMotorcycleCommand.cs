using Application.DTOs;
using MediatR;

namespace Application.Features.Motorcycles.Commands.CreateMotorcycle;

public record CreateMotorcycleCommand(CreateMotorcycleDto Dto) : IRequest<MotorcycleDto>;
