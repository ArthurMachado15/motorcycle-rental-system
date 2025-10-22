using Application.Common.Exceptions;
using Application.DTOs;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Couriers.Commands.CreateCourier;

public class CreateCourierCommandHandler : IRequestHandler<CreateCourierCommand, CourierDto>
{
    private readonly ICourierRepository _courierRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateCourierCommandHandler(
        ICourierRepository courierRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _courierRepository = courierRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CourierDto> Handle(CreateCourierCommand request, CancellationToken cancellationToken)
    {
        // Validate driver license type
        var validTypes = new[] { "A", "B", "A+B", "AB" };
        if (!validTypes.Contains(request.Dto.TipoCnh.ToUpper()))
        {
            throw new ValidationException("Dados inválidos");
        }

        // Check if CNPJ already exists
        var existingByCnpj = await _courierRepository.GetByCNPJAsync(request.Dto.Cnpj, cancellationToken);
        if (existingByCnpj != null)
        {
            throw new ValidationException("Dados inválidos");
        }

        // Check if driver license number already exists
        var existingByLicense = await _courierRepository.GetByDriverLicenseNumberAsync(request.Dto.NumeroCnh, cancellationToken);
        if (existingByLicense != null)
        {
            throw new ValidationException("Dados inválidos");
        }

        var courier = _mapper.Map<Courier>(request.Dto);
        courier.Id = Guid.NewGuid();

        await _courierRepository.AddAsync(courier, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CourierDto>(courier);
    }
}
