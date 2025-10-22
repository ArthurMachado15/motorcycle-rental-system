using Application.Common.Exceptions;
using Application.DTOs;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Motorcycles.Queries.GetMotorcycleByIdentifier;

public class GetMotorcycleByIdentifierQueryHandler : IRequestHandler<GetMotorcycleByIdentifierQuery, MotorcycleDto>
{
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IMapper _mapper;

    public GetMotorcycleByIdentifierQueryHandler(
        IMotorcycleRepository motorcycleRepository,
        IMapper mapper)
    {
        _motorcycleRepository = motorcycleRepository;
        _mapper = mapper;
    }

    public async Task<MotorcycleDto> Handle(GetMotorcycleByIdentifierQuery request, CancellationToken cancellationToken)
    {
        var motorcycle = await _motorcycleRepository.GetByIdentifierAsync(request.Identifier, cancellationToken);

        if (motorcycle == null)
            throw new NotFoundException(nameof(motorcycle), request.Identifier);

        return _mapper.Map<MotorcycleDto>(motorcycle);
    }
}
