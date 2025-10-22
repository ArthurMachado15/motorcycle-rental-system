using Application.Common.Exceptions;
using Application.DTOs;
using Application.Features.Couriers.Commands.CreateCourier;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace UnitTests.Application;

public class CreateCourierCommandHandlerTests
{
    private readonly Mock<ICourierRepository> _courierRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateCourierCommandHandler _handler;

    public CreateCourierCommandHandlerTests()
    {
        _courierRepositoryMock = new Mock<ICourierRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();

        _handler = new CreateCourierCommandHandler(
            _courierRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _mapperMock.Object);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("B")]
    [InlineData("A+B")]
    [InlineData("AB")]
    public async Task Handle_ValidDriverLicenseType_ShouldCreateSuccessfully(string licenseType)
    {
        // Arrange
        var dto = new CreateCourierDto
        {
            Identificador = $"courier-{licenseType}",
            Nome = "John Doe",
            Cnpj = "12345678901234",
            DataNascimento = new DateTime(1990, 1, 1),
            NumeroCnh = "12345678901",
            TipoCnh = licenseType
        };

        var courier = new Courier
        {
            Id = Guid.NewGuid(),
            Name = dto.Nome,
            CNPJ = dto.Cnpj,
            BirthDate = DateOnly.FromDateTime(dto.DataNascimento),
            DriverLicenseNumber = dto.NumeroCnh,
            DriverLicenseType = DriverLicenseType.A
        };

        var courierDto = new CourierDto
        {
            Identificador = courier.Id.ToString(),
            Nome = courier.Name,
            Cnpj = courier.CNPJ,
            DataNascimento = courier.BirthDate.ToDateTime(TimeOnly.MinValue),
            NumeroCnh = courier.DriverLicenseNumber,
            TipoCnh = licenseType
        };

        _courierRepositoryMock
            .Setup(x => x.GetByCNPJAsync(dto.Cnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Courier?)null);

        _courierRepositoryMock
            .Setup(x => x.GetByDriverLicenseNumberAsync(dto.NumeroCnh, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Courier?)null);

        _mapperMock
            .Setup(x => x.Map<Courier>(dto))
            .Returns(courier);

        _mapperMock
            .Setup(x => x.Map<CourierDto>(courier))
            .Returns(courierDto);

        var command = new CreateCourierCommand(dto);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Nome.Should().Be(dto.Nome);
        result.Cnpj.Should().Be(dto.Cnpj);

        _courierRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Courier>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidDriverLicenseType_ShouldThrowValidationException()
    {
        // Arrange
        var dto = new CreateCourierDto
        {
            Identificador = "courier-invalid",
            Nome = "John Doe",
            Cnpj = "12345678901234",
            DataNascimento = new DateTime(1990, 1, 1),
            NumeroCnh = "12345678901",
            TipoCnh = "C" // Invalid
        };

        var command = new CreateCourierCommand(dto);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Dados inválidos");
    }

    [Fact]
    public async Task Handle_DuplicateCNPJ_ShouldThrowValidationException()
    {
        // Arrange
        var dto = new CreateCourierDto
        {
            Identificador = "courier-dup-cnpj",
            Nome = "John Doe",
            Cnpj = "12345678901234",
            DataNascimento = new DateTime(1990, 1, 1),
            NumeroCnh = "12345678901",
            TipoCnh = "A"
        };

        var existingCourier = new Courier { Id = Guid.NewGuid(), CNPJ = dto.Cnpj };

        _courierRepositoryMock
            .Setup(x => x.GetByCNPJAsync(dto.Cnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCourier);

        var command = new CreateCourierCommand(dto);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Dados inválidos");
    }

    [Fact]
    public async Task Handle_DuplicateDriverLicenseNumber_ShouldThrowValidationException()
    {
        // Arrange
        var dto = new CreateCourierDto
        {
            Identificador = "courier-dup-cnh",
            Nome = "John Doe",
            Cnpj = "12345678901234",
            DataNascimento = new DateTime(1990, 1, 1),
            NumeroCnh = "12345678901",
            TipoCnh = "A"
        };

        var existingCourier = new Courier { Id = Guid.NewGuid(), DriverLicenseNumber = dto.NumeroCnh };

        _courierRepositoryMock
            .Setup(x => x.GetByCNPJAsync(dto.Cnpj, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Courier?)null);

        _courierRepositoryMock
            .Setup(x => x.GetByDriverLicenseNumberAsync(dto.NumeroCnh, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCourier);

        var command = new CreateCourierCommand(dto);

        // Act
        Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>()
            .WithMessage("Dados inválidos");
    }
}
