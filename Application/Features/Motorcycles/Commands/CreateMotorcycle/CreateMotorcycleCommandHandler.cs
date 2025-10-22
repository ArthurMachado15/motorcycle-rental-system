using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Motorcycles.Commands.CreateMotorcycle;

public class CreateMotorcycleCommandHandler : IRequestHandler<CreateMotorcycleCommand, MotorcycleDto>
{
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IMessagePublisher _messagePublisher;

    public CreateMotorcycleCommandHandler(
        IMotorcycleRepository motorcycleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IMessagePublisher messagePublisher)
    {
        _motorcycleRepository = motorcycleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _messagePublisher = messagePublisher;
    }

    public async Task<MotorcycleDto> Handle(CreateMotorcycleCommand request, CancellationToken cancellationToken)
    {
        // Check if license plate already exists
        var existing = await _motorcycleRepository.GetByLicensePlateAsync(request.Dto.Placa, cancellationToken);
        if (existing != null)
        {
            throw new ValidationException("Dados inválidos");
        }

        var motorcycle = _mapper.Map<Motorcycle>(request.Dto);
        motorcycle.Id = Guid.NewGuid();

        await _motorcycleRepository.AddAsync(motorcycle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Publish event
        var eventMessage = new
        {
            motorcycleId = motorcycle.Id,
            year = motorcycle.Year,
            model = motorcycle.Model,
            licensePlate = motorcycle.LicensePlate,
            registeredAt = DateTime.UtcNow
        };

        await _messagePublisher.PublishAsync("motorcycle-registered", eventMessage, cancellationToken);

        return _mapper.Map<MotorcycleDto>(motorcycle);
    }
}
