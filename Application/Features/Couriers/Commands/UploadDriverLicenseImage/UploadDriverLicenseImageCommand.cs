using MediatR;

namespace Application.Features.Couriers.Commands.UploadDriverLicenseImage;

public record UploadDriverLicenseImageCommand(
    Guid CourierId, 
    Stream FileStream, 
    string FileName,
    string ContentType) : IRequest<Unit>;
