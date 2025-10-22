using Application.DTOs;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Motorcycles.Queries.GetMotorcycles;

public class GetMotorcyclesQueryHandler : IRequestHandler<GetMotorcyclesQuery, IEnumerable<MotorcycleDto>>
{
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IMapper _mapper;

    public GetMotorcyclesQueryHandler(IMotorcycleRepository motorcycleRepository, IMapper mapper)
    {
        _motorcycleRepository = motorcycleRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MotorcycleDto>> Handle(GetMotorcyclesQuery request, CancellationToken cancellationToken)
    {
        var motorcycles = await _motorcycleRepository.GetAllAsync(request.LicensePlate, cancellationToken);
        return _mapper.Map<IEnumerable<MotorcycleDto>>(motorcycles);
    }
}
