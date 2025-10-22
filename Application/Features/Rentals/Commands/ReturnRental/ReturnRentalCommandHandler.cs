using Application.Common.Exceptions;
using Application.DTOs;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Rentals.Commands.ReturnRental;

public class ReturnRentalCommandHandler : IRequestHandler<ReturnRentalCommand, RentalDto>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ReturnRentalCommandHandler(
        IRentalRepository rentalRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _rentalRepository = rentalRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RentalDto> Handle(ReturnRentalCommand request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.RentalId, cancellationToken);
        if (rental == null)
        {
            throw new NotFoundException(nameof(rental), request.RentalId);
        }

        if (rental.ActualEndDate != null)
        {
            throw new ValidationException("Rental has already been returned");
        }

        rental.ActualEndDate = request.ReturnDate;
        rental.CalculateTotalCost();

        await _rentalRepository.UpdateAsync(rental, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<RentalDto>(rental);
    }
}
