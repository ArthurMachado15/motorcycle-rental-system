using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.Couriers.Commands.UploadDriverLicenseImage;

public class UploadDriverLicenseImageCommandHandler : IRequestHandler<UploadDriverLicenseImageCommand, Unit>
{
    private readonly ICourierRepository _courierRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorage _fileStorage;

    public UploadDriverLicenseImageCommandHandler(
        ICourierRepository courierRepository,
        IUnitOfWork unitOfWork,
        IFileStorage fileStorage)
    {
        _courierRepository = courierRepository;
        _unitOfWork = unitOfWork;
        _fileStorage = fileStorage;
    }

    public async Task<Unit> Handle(UploadDriverLicenseImageCommand request, CancellationToken cancellationToken)
    {
        // Validate file extension
        var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
        if (extension != ".png" && extension != ".bmp")
        {
            throw new ValidationException("Only PNG and BMP files are allowed");
        }

        var courier = await _courierRepository.GetByIdAsync(request.CourierId, cancellationToken);
        if (courier == null)
        {
            throw new NotFoundException(nameof(courier), request.CourierId);
        }

        // Delete old file if exists
        if (!string.IsNullOrEmpty(courier.DriverLicenseImagePath))
        {
            await _fileStorage.DeleteFileAsync(courier.DriverLicenseImagePath, cancellationToken);
        }

        // Save new file
        var filePath = await _fileStorage.SaveFileAsync(request.FileStream, request.FileName, cancellationToken);
        
        courier.DriverLicenseImagePath = filePath;
        await _courierRepository.UpdateAsync(courier, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
