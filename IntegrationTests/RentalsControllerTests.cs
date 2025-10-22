using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;
using Xunit;

namespace IntegrationTests;

public class RentalsControllerTests : IntegrationTestBase
{
    public RentalsControllerTests()
    {
    }

    [Fact]
    public async Task CreateRental_ValidData_ReturnsCreated()
    {
        // Arrange - Create Motorcycle
        var motorcycleDto = new CreateMotorcycleDto
        {
            Identificador = "moto-rental-test-1",
            Ano = 2024,
            Modelo = "Honda CG 160",
            Placa = "RNT-1111"
        };
        var motoResponse = await _client.PostAsJsonAsync("/motos", motorcycleDto);
        motoResponse.EnsureSuccessStatusCode();

        // Arrange - Create Courier
        var courierDto = new CreateCourierDto
        {
            Identificador = "courier-rental-test-1",
            Nome = "João da Silva",
            Cnpj = "12345678901234",
            DataNascimento = new DateTime(1990, 1, 1),
            NumeroCnh = "12345678900",
            TipoCnh = "A"
        };
        var courierResponse = await _client.PostAsJsonAsync("/entregadores", courierDto);
        courierResponse.EnsureSuccessStatusCode();
        var createdCourier = await courierResponse.Content.ReadFromJsonAsync<CourierDto>();

        // Arrange - Rental DTO
        var rentalDto = new CreateRentalDto
        {
            EntregadorId = createdCourier!.Identificador,
            MotoId = motorcycleDto.Identificador,
            DataInicio = DateTime.Today.AddDays(1),
            DataTermino = DateTime.Today.AddDays(8),
            DataPrevisaoTermino = DateTime.Today.AddDays(8),
            Plano = 7
        };

        // Act
        var response = await _client.PostAsJsonAsync("/locacao", rentalDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<RentalDto>();
        result.Should().NotBeNull();
        result!.EntregadorId.Should().Be(createdCourier.Identificador);
        result.MotoId.Should().Be(motorcycleDto.Identificador);
    }

    [Fact]
    public async Task CreateRental_CourierWithoutTypeA_ReturnsBadRequest()
    {
        // Arrange - Create Motorcycle
        var motorcycleDto = new CreateMotorcycleDto
        {
            Identificador = "moto-rental-test-2",
            Ano = 2024,
            Modelo = "Honda PCX",
            Placa = "RNT-2222"
        };
        await _client.PostAsJsonAsync("/motos", motorcycleDto);

        // Arrange - Create Courier with Type B
        var courierDto = new CreateCourierDto
        {
            Identificador = "courier-rental-test-2",
            Nome = "Maria Santos",
            Cnpj = "98765432109876",
            DataNascimento = new DateTime(1995, 5, 15),
            NumeroCnh = "98765432100",
            TipoCnh = "B" // Not A or A+B
        };
        var courierResponse = await _client.PostAsJsonAsync("/entregadores", courierDto);
        var createdCourier = await courierResponse.Content.ReadFromJsonAsync<CourierDto>();

        // Arrange - Rental DTO
        var rentalDto = new CreateRentalDto
        {
            EntregadorId = createdCourier!.Identificador,
            MotoId = motorcycleDto.Identificador,
            DataInicio = DateTime.Today.AddDays(1),
            DataTermino = DateTime.Today.AddDays(8),
            DataPrevisaoTermino = DateTime.Today.AddDays(8),
            Plano = 7
        };

        // Act
        var response = await _client.PostAsJsonAsync("/locacao", rentalDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetRentalById_ExistingId_ReturnsOk()
    {
        // Arrange - Create Motorcycle
        var motorcycleDto = new CreateMotorcycleDto
        {
            Identificador = "moto-rental-test-3",
            Ano = 2024,
            Modelo = "Yamaha Factor",
            Placa = "RNT-3333"
        };
        await _client.PostAsJsonAsync("/motos", motorcycleDto);

        // Arrange - Create Courier
        var courierDto = new CreateCourierDto
        {
            Identificador = "courier-rental-test-3",
            Nome = "Pedro Costa",
            Cnpj = "11111111111111",
            DataNascimento = new DateTime(1988, 3, 20),
            NumeroCnh = "11111111111",
            TipoCnh = "A"
        };
        var courierResponse = await _client.PostAsJsonAsync("/entregadores", courierDto);
        var createdCourier = await courierResponse.Content.ReadFromJsonAsync<CourierDto>();

        // Arrange - Create Rental
        var rentalDto = new CreateRentalDto
        {
            EntregadorId = createdCourier!.Identificador,
            MotoId = motorcycleDto.Identificador,
            DataInicio = DateTime.Today.AddDays(1),
            DataTermino = DateTime.Today.AddDays(15),
            DataPrevisaoTermino = DateTime.Today.AddDays(15),
            Plano = 15
        };
        var createResponse = await _client.PostAsJsonAsync("/locacao", rentalDto);
        var createdRental = await createResponse.Content.ReadFromJsonAsync<RentalDto>();

        // Act
        var response = await _client.GetAsync($"/locacao/{createdRental!.Identificador}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<RentalDto>();
        result.Should().NotBeNull();
        result!.Identificador.Should().Be(createdRental.Identificador);
    }

    [Fact]
    public async Task GetRentalById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/locacao/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReturnRental_ValidData_ReturnsOk()
    {
        // Arrange - Create Motorcycle
        var motorcycleDto = new CreateMotorcycleDto
        {
            Identificador = "moto-rental-test-4",
            Ano = 2024,
            Modelo = "Suzuki Intruder",
            Placa = "RNT-4444"
        };
        await _client.PostAsJsonAsync("/motos", motorcycleDto);

        // Arrange - Create Courier
        var courierDto = new CreateCourierDto
        {
            Identificador = "courier-rental-test-4",
            Nome = "Ana Paula",
            Cnpj = "22222222222222",
            DataNascimento = new DateTime(1992, 7, 10),
            NumeroCnh = "22222222222",
            TipoCnh = "A+B"
        };
        var courierResponse = await _client.PostAsJsonAsync("/entregadores", courierDto);
        var createdCourier = await courierResponse.Content.ReadFromJsonAsync<CourierDto>();

        // Arrange - Create Rental
        var rentalDto = new CreateRentalDto
        {
            EntregadorId = createdCourier!.Identificador,
            MotoId = motorcycleDto.Identificador,
            DataInicio = DateTime.Today,
            DataTermino = DateTime.Today.AddDays(30),
            DataPrevisaoTermino = DateTime.Today.AddDays(30),
            Plano = 30
        };
        var createResponse = await _client.PostAsJsonAsync("/locacao", rentalDto);
        var createdRental = await createResponse.Content.ReadFromJsonAsync<RentalDto>();

        // Arrange - Return DTO
        var returnDto = new ReturnRentalDto
        {
            DataDevolucao = DateTime.Today.AddDays(30)
        };

        // Act
        var response = await _client.PutAsJsonAsync($"/locacao/{createdRental!.Identificador}/devolucao", returnDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
