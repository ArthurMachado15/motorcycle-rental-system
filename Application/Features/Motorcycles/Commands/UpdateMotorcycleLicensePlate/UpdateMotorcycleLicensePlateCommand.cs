using MediatR;

namespace Application.Features.Motorcycles.Commands.UpdateMotorcycleLicensePlate;

public record UpdateMotorcycleLicensePlateCommand(Guid Id, string LicensePlate) : IRequest<Unit>;
