using Application.Common.Exceptions;
using Application.DTOs;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Rentals.Queries.GetRentalById;

public class GetRentalByIdQueryHandler : IRequestHandler<GetRentalByIdQuery, RentalDto>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly IMapper _mapper;

    public GetRentalByIdQueryHandler(IRentalRepository rentalRepository, IMapper mapper)
    {
        _rentalRepository = rentalRepository;
        _mapper = mapper;
    }

    public async Task<RentalDto> Handle(GetRentalByIdQuery request, CancellationToken cancellationToken)
    {
        var rental = await _rentalRepository.GetByIdAsync(request.Id, cancellationToken);
        if (rental == null)
        {
            throw new NotFoundException(nameof(rental), request.Id);
        }

        return _mapper.Map<RentalDto>(rental);
    }
}
