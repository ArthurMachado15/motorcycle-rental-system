using Application.DTOs;
using Application.Features.Rentals.Commands.CreateRental;
using Application.Features.Rentals.Commands.ReturnRental;
using Application.Features.Rentals.Queries.GetRentalById;
using Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("locacao")]
public class RentalsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IRentalRepository _rentalRepository;

    public RentalsController(IMediator mediator, IRentalRepository rentalRepository)
    {
        _mediator = mediator;
        _rentalRepository = rentalRepository;
    }

    /// <summary>
    /// Alugar uma moto
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RentalDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateRentalDto dto)
    {
        var command = new CreateRentalCommand(dto);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result.Identificador }, result);
    }

    /// <summary>
    /// Consultar locação por id
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RentalDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(string id)
    {
        var rentalId = await ResolveRentalIdAsync(id);
        if (rentalId == Guid.Empty)
            return BadRequest(new ErrorResponse { Mensagem = "Dados inválidos" });
            
        var query = new GetRentalByIdQuery(rentalId);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Informar data de devolução e calcular valor
    /// </summary>
    [HttpPut("{id}/devolucao")]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Return(string id, [FromBody] ReturnRentalDto dto)
    {
        var rentalId = await ResolveRentalIdAsync(id);
        if (rentalId == Guid.Empty)
            return BadRequest(new ErrorResponse { Mensagem = "Dados inválidos" });
            
        var command = new ReturnRentalCommand(rentalId, DateOnly.FromDateTime(dto.DataDevolucao));
        await _mediator.Send(command);
        return Ok(new ErrorResponse { Mensagem = "Data de devolução informada com sucesso" });
    }

    private async Task<Guid> ResolveRentalIdAsync(string id)
    {
        if (Guid.TryParse(id, out var rentalId))
            return rentalId;

        var rental = await _rentalRepository.GetByIdentifierAsync(id);
        return rental?.Id ?? Guid.Empty;
    }
}
