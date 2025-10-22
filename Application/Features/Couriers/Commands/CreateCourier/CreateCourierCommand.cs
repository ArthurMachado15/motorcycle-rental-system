using Application.DTOs;
using MediatR;

namespace Application.Features.Couriers.Commands.CreateCourier;

public record CreateCourierCommand(CreateCourierDto Dto) : IRequest<CourierDto>;
