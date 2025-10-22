using Application.Common.Exceptions;
using Application.DTOs;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Motorcycles.Queries.GetMotorcycleById;

public class GetMotorcycleByIdQueryHandler : IRequestHandler<GetMotorcycleByIdQuery, MotorcycleDto>
{
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IMapper _mapper;

    public GetMotorcycleByIdQueryHandler(IMotorcycleRepository motorcycleRepository, IMapper mapper)
    {
        _motorcycleRepository = motorcycleRepository;
        _mapper = mapper;
    }

    public async Task<MotorcycleDto> Handle(GetMotorcycleByIdQuery request, CancellationToken cancellationToken)
    {
        var motorcycle = await _motorcycleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (motorcycle == null)
        {
            throw new NotFoundException(nameof(motorcycle), request.Id);
        }

        return _mapper.Map<MotorcycleDto>(motorcycle);
    }
}
