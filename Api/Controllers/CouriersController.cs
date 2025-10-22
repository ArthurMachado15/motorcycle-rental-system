using Application.DTOs;
using Application.Features.Couriers.Commands.CreateCourier;
using Application.Features.Couriers.Commands.UploadDriverLicenseImage;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("entregadores")]
public class CouriersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICourierRepository _courierRepository;

    public CouriersController(IMediator mediator, ICourierRepository courierRepository)
    {
        _mediator = mediator;
        _courierRepository = courierRepository;
    }

    /// <summary>
    /// Cadastrar entregador
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CourierDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCourierDto dto)
    {
        var command = new CreateCourierCommand(dto);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(Create), new { id = result.Identificador }, result);
    }

    /// <summary>
    /// Enviar foto da CNH
    /// </summary>
    [HttpPost("{id}/cnh")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadCnhImage(string id, [FromBody] UploadCnhImageDto dto)
    {
        var courierId = await ResolveCourierIdAsync(id);
        if (courierId == Guid.Empty)
            return BadRequest(new ErrorResponse { Mensagem = "Dados inválidos" });

        if (string.IsNullOrWhiteSpace(dto.ImagemCnh))
            return BadRequest(new ErrorResponse { Mensagem = "Dados inválidos" });

        try
        {
            // Decode base64 string
            var imageBytes = Convert.FromBase64String(dto.ImagemCnh);
            using var stream = new MemoryStream(imageBytes);
            
            var command = new UploadDriverLicenseImageCommand(courierId, stream, $"cnh_{courierId}.png", "image/png");
            await _mediator.Send(command);
            
            return StatusCode(201);
        }
        catch
        {
            return BadRequest(new ErrorResponse { Mensagem = "Dados inválidos" });
        }
    }

    private async Task<Guid> ResolveCourierIdAsync(string id)
    {
        if (Guid.TryParse(id, out var courierId))
            return courierId;

        var courier = await _courierRepository.GetByIdentifierAsync(id);
        return courier?.Id ?? Guid.Empty;
    }
}
