using Application.Common.Exceptions;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Motorcycles.Commands.DeleteMotorcycle;

public class DeleteMotorcycleCommandHandler : IRequestHandler<DeleteMotorcycleCommand, Unit>
{
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteMotorcycleCommandHandler(
        IMotorcycleRepository motorcycleRepository,
        IUnitOfWork unitOfWork)
    {
        _motorcycleRepository = motorcycleRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteMotorcycleCommand request, CancellationToken cancellationToken)
    {
        var motorcycle = await _motorcycleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (motorcycle == null)
        {
            throw new NotFoundException(nameof(motorcycle), request.Id);
        }

        // Check if motorcycle has rental history
        var hasRentalHistory = await _motorcycleRepository.HasRentalHistoryAsync(request.Id, cancellationToken);
        if (hasRentalHistory)
        {
            throw new ValidationException("Cannot delete motorcycle with rental history");
        }

        await _motorcycleRepository.DeleteAsync(motorcycle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
