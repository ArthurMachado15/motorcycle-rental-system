using MediatR;

namespace Application.Features.Motorcycles.Commands.DeleteMotorcycle;

public record DeleteMotorcycleCommand(Guid Id) : IRequest<Unit>;
