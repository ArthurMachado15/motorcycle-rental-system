using Application.Common.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Motorcycles.Commands.UpdateMotorcycleLicensePlate;

public class UpdateMotorcycleLicensePlateCommandHandler : IRequestHandler<UpdateMotorcycleLicensePlateCommand, Unit>
{
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateMotorcycleLicensePlateCommandHandler(
        IMotorcycleRepository motorcycleRepository,
        IUnitOfWork unitOfWork)
    {
        _motorcycleRepository = motorcycleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateMotorcycleLicensePlateCommand request, CancellationToken cancellationToken)
    {
        var motorcycle = await _motorcycleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (motorcycle == null)
        {
            throw new NotFoundException(nameof(motorcycle), request.Id);
        }

        // Check if new license plate already exists
        var existing = await _motorcycleRepository.GetByLicensePlateAsync(request.LicensePlate, cancellationToken);
        if (existing != null && existing.Id != request.Id)
        {
            throw new ValidationException("License plate already exists");
        }

        motorcycle.LicensePlate = request.LicensePlate;
        await _motorcycleRepository.UpdateAsync(motorcycle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
