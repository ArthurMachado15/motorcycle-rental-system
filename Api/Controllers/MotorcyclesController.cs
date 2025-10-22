using Application.DTOs;
using Application.Features.Motorcycles.Commands.CreateMotorcycle;
using Application.Features.Motorcycles.Commands.DeleteMotorcycle;
using Application.Features.Motorcycles.Commands.UpdateMotorcycleLicensePlate;
using Application.Features.Motorcycles.Queries.GetMotorcycleById;
using Application.Features.Motorcycles.Queries.GetMotorcycleByIdentifier;
using Application.Features.Motorcycles.Queries.GetMotorcycles;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("motos")]
public class MotorcyclesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMotorcycleRepository _motorcycleRepository;

    public MotorcyclesController(IMediator mediator, IMotorcycleRepository motorcycleRepository)
    {
        _mediator = mediator;
        _motorcycleRepository = motorcycleRepository;
    }

    /// <summary>
    /// Cadastrar uma nova moto
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MotorcycleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateMotorcycleDto dto)
    {
        var command = new CreateMotorcycleCommand(dto);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Identificador }, result);
    }

    /// <summary>
    /// Consultar motos existentes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<MotorcycleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? placa = null)
    {
        var query = new GetMotorcyclesQuery(placa);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Consultar motos existentes por id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(MotorcycleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        // Try as Guid first, fallback to Identifier
        if (Guid.TryParse(id, out var motorcycleId))
        {
            var query = new GetMotorcycleByIdQuery(motorcycleId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
        
        // Search by Identifier
        var queryByIdentifier = new GetMotorcycleByIdentifierQuery(id);
        var resultByIdentifier = await _mediator.Send(queryByIdentifier);
        return Ok(resultByIdentifier);
    }

    /// <summary>
    /// Modificar a placa de uma moto
    /// </summary>
    [HttpPut("{id}/placa")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateLicensePlate(string id, [FromBody] UpdateMotorcycleLicensePlateDto dto)
    {
        var motorcycleId = await ResolveMotorcycleIdAsync(id);
        if (motorcycleId == Guid.Empty)
            return BadRequest(new ErrorResponse { Mensagem = "Dados inválidos" });
            
        var command = new UpdateMotorcycleLicensePlateCommand(motorcycleId, dto.Placa);
        await _mediator.Send(command);
        return Ok(new ErrorResponse { Mensagem = "Placa modificada com sucesso" });
    }

    /// <summary>
    /// Remover uma moto
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(string id)
    {
        var motorcycleId = await ResolveMotorcycleIdAsync(id);
        if (motorcycleId == Guid.Empty)
            return BadRequest(new ErrorResponse { Mensagem = "Dados inválidos" });
            
        var command = new DeleteMotorcycleCommand(motorcycleId);
        await _mediator.Send(command);
        return Ok();
    }

    private async Task<Guid> ResolveMotorcycleIdAsync(string id)
    {
        if (Guid.TryParse(id, out var motorcycleId))
            return motorcycleId;

        var motorcycle = await _motorcycleRepository.GetByIdentifierAsync(id);
        return motorcycle?.Id ?? Guid.Empty;
    }
}
