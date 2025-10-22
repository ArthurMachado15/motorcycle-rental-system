using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.DTOs;
using Application.Features.Motorcycles.Commands.CreateMotorcycle;
using AutoMapper;
using Domain.Entities;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace UnitTests.Application;

public class CreateMotorcycleCommandHandlerTests
{
    private readonly Mock<IMotorcycleRepository> _motorcycleRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMessagePublisher> _messagePublisherMock;
    private readonly CreateMotorcycleCommandHandler _handler;

    public CreateMotorcycleCommandHandlerTests()
    {
        _motorcycleRepositoryMock = new Mock<IMotorcycleRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _messagePublisherMock = new Mock<IMessagePublisher>();

        _handler = new CreateMotorcycleCommandHandler(
            _motorcycleRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _messagePublisherMock.Object);
    }

    [Fact]
    public async Task Handle_ValidMotorcycle_ShouldCreateSuccessfully()
    {
        // Arrange
        var dto = new CreateMotorcycleDto
        {
            Identificador = "moto-unit-test",
            Ano = 2024,
            Modelo = "Honda CG 160",
            Placa = "ABC1234"
        };

        var motorcycle = new Motorcycle
        {
            Id = Guid.NewGuid(),
            Year = dto.Ano,
            Model = dto.Modelo,
            LicensePlate = dto.Placa
        };

        var motorcycleDto = new MotorcycleDto
        {
            Identificador = motorcycle.Id.ToString(),
            Ano = motorcycle.Year,
            Modelo = motorcycle.Model,
            Placa = motorcycle.LicensePlate
        };

        _motorcycleRepositoryMock
            .Setup(x => x.GetByLicensePlateAsync(dto.Placa, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Motorcycle?)null);

        _mapperMock
            .Setup(x => x.Map<Motorcycle>(dto))
            .Returns(motorcycle);

        _mapperMock
            .Setup(x => x.Map<MotorcycleDto>(motorcycle))
            .Returns(motorcycleDto);

        _motorcycleRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Motorcycle>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(motorcycle);

        var command = new CreateMotorcycleCommand(dto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Placa.Should().Be(dto.Placa);
        result.Modelo.Should().Be(dto.Modelo);
        result.Ano.Should().Be(dto.Ano);

        _motorcycleRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Motorcycle>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);

        _messagePublisherMock.Verify(
            x => x.PublishAsync("motorcycle-registered", It.IsAny<object>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_DuplicateLicensePlate_ShouldThrowValidationException()
    {
        // Arrange
        var dto = new CreateMotorcycleDto
        {
            Identificador = "moto-unit-test",
            Ano = 2024,
            Modelo = "Honda CG 160",
            Placa = "ABC1234"
        };

        var existingMotorcycle = new Motorcycle
        {
            Id = Guid.NewGuid(),
            LicensePlate = dto.Placa
        };

        _motorcycleRepositoryMock
            .Setup(x => x.GetByLicensePlateAsync(dto.Placa, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingMotorcycle);

        var command = new CreateMotorcycleCommand(dto);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Dados inválidos");
    }
}
