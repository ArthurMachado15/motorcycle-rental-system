using Application.Common.Exceptions;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Rentals.Commands.CreateRental;

public class CreateRentalCommandHandler : IRequestHandler<CreateRentalCommand, RentalDto>
{
    private readonly IRentalRepository _rentalRepository;
    private readonly ICourierRepository _courierRepository;
    private readonly IMotorcycleRepository _motorcycleRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateRentalCommandHandler(
        IRentalRepository rentalRepository,
        ICourierRepository courierRepository,
        IMotorcycleRepository motorcycleRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _rentalRepository = rentalRepository;
        _courierRepository = courierRepository;
        _motorcycleRepository = motorcycleRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<RentalDto> Handle(CreateRentalCommand request, CancellationToken cancellationToken)
    {
        Courier? courier = null;
        if (Guid.TryParse(request.Dto.EntregadorId, out var courierId))
            courier = await _courierRepository.GetByIdAsync(courierId, cancellationToken);
        else
            courier = await _courierRepository.GetByIdentifierAsync(request.Dto.EntregadorId, cancellationToken);

        if (courier == null)
            throw new NotFoundException(nameof(Courier), request.Dto.EntregadorId);

        if (courier.DriverLicenseType != DriverLicenseType.A && courier.DriverLicenseType != DriverLicenseType.AB)
            throw new ValidationException("Dados inválidos");

        Motorcycle? motorcycle = null;
        if (Guid.TryParse(request.Dto.MotoId, out var motorcycleId))
            motorcycle = await _motorcycleRepository.GetByIdAsync(motorcycleId, cancellationToken);
        else
            motorcycle = await _motorcycleRepository.GetByIdentifierAsync(request.Dto.MotoId, cancellationToken);

        if (motorcycle == null)
            throw new NotFoundException(nameof(Motorcycle), request.Dto.MotoId);

        var hasActiveRental = await _rentalRepository.HasActiveCourierRentalAsync(courier.Id, cancellationToken);
        if (hasActiveRental)
            throw new ValidationException("Dados inválidos");

        var validPlans = new[] { 7, 15, 30, 45, 50 };
        if (!validPlans.Contains(request.Dto.Plano))
            throw new ValidationException("Dados inválidos");

        var rental = _mapper.Map<Rental>(request.Dto);
        rental.Id = Guid.NewGuid();
        rental.Identifier = $"rental-{Guid.NewGuid():N}"[..20];
        rental.CourierId = courier.Id;
        rental.MotorcycleId = motorcycle.Id;
        rental.Courier = courier;
        rental.Motorcycle = motorcycle;
        rental.StartDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));
        rental.ExpectedEndDate = rental.StartDate.AddDays(request.Dto.Plano);
        rental.DailyCost = GetDailyCost(rental.PlanType);
        rental.CalculateTotalCost();

        await _rentalRepository.AddAsync(rental, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<RentalDto>(rental);
    }

    private static decimal GetDailyCost(RentalPlanType planType)
    {
        return planType switch
        {
            RentalPlanType.SevenDays => 30.00m,
            RentalPlanType.FifteenDays => 28.00m,
            RentalPlanType.ThirtyDays => 22.00m,
            RentalPlanType.FortyFiveDays => 20.00m,
            RentalPlanType.FiftyDays => 18.00m,
            _ => throw new ArgumentException("Invalid plan type")
        };
    }
}
