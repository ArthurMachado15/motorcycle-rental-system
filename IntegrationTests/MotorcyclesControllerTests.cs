using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;
using Xunit;

namespace IntegrationTests;

public class MotorcyclesControllerTests : IntegrationTestBase
{
    public MotorcyclesControllerTests()
    {
    }

    [Fact]
    public async Task CreateMotorcycle_ValidData_ReturnsCreated()
    {
        // Arrange
        var createDto = new CreateMotorcycleDto
        {
            Identificador = "moto-test-1",
            Ano = 2024,
            Modelo = "Honda CG 160",
            Placa = "ABC1234"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/motos", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<MotorcycleDto>();
        result.Should().NotBeNull();
        result!.Ano.Should().Be(createDto.Ano);
        result.Modelo.Should().Be(createDto.Modelo);
        result.Placa.Should().Be(createDto.Placa);
    }

    [Fact]
    public async Task CreateMotorcycle_DuplicateLicensePlate_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateMotorcycleDto
        {
            Identificador = "moto-test-2",
            Ano = 2024,
            Modelo = "Honda CG 160",
            Placa = "XYZ9999"
        };

        // Create first motorcycle
        await _client.PostAsJsonAsync("/motos", createDto);

        // Act - Try to create with same license plate
        createDto.Identificador = "moto-test-2-dup";
        var response = await _client.PostAsJsonAsync("/motos", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetAllMotorcycles_ReturnsOk()
    {
        // Arrange
        var createDto = new CreateMotorcycleDto
        {
            Identificador = "moto-test-3",
            Ano = 2023,
            Modelo = "Yamaha YBR 125",
            Placa = "DEF5678"
        };
        await _client.PostAsJsonAsync("/motos", createDto);

        // Act
        var response = await _client.GetAsync("/motos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<IEnumerable<MotorcycleDto>>();
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetMotorcycleById_ExistingId_ReturnsOk()
    {
        // Arrange
        var createDto = new CreateMotorcycleDto
        {
            Identificador = "moto-test-4",
            Ano = 2024,
            Modelo = "Honda PCX",
            Placa = "GHI1111"
        };
        var createResponse = await _client.PostAsJsonAsync("/motos", createDto);
        var createdMotorcycle = await createResponse.Content.ReadFromJsonAsync<MotorcycleDto>();

        // Act
        var response = await _client.GetAsync($"/motos/{createdMotorcycle!.Identificador}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<MotorcycleDto>();
        result.Should().NotBeNull();
        result!.Identificador.Should().Be(createdMotorcycle.Identificador);
    }

    [Fact]
    public async Task GetMotorcycleById_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/motos/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateMotorcycleLicensePlate_ValidData_ReturnsOk()
    {
        // Arrange
        var createDto = new CreateMotorcycleDto
        {
            Identificador = "moto-test-5",
            Ano = 2024,
            Modelo = "Suzuki GSX",
            Placa = "JKL2222"
        };
        var createResponse = await _client.PostAsJsonAsync("/motos", createDto);
        var createdMotorcycle = await createResponse.Content.ReadFromJsonAsync<MotorcycleDto>();

        var updateDto = new UpdateMotorcycleLicensePlateDto
        {
            Placa = "JKL2223"
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            $"/motos/{createdMotorcycle!.Identificador}/placa",
            updateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify the update
        var getResponse = await _client.GetAsync($"/motos/{createdMotorcycle.Identificador}");
        var updatedMotorcycle = await getResponse.Content.ReadFromJsonAsync<MotorcycleDto>();
        updatedMotorcycle!.Placa.Should().Be(updateDto.Placa);
    }

    [Fact]
    public async Task DeleteMotorcycle_NoRentalHistory_ReturnsOk()
    {
        // Arrange
        var createDto = new CreateMotorcycleDto
        {
            Identificador = "moto-test-6",
            Ano = 2024,
            Modelo = "Kawasaki Ninja",
            Placa = "MNO3333"
        };
        var createResponse = await _client.PostAsJsonAsync("/motos", createDto);
        var createdMotorcycle = await createResponse.Content.ReadFromJsonAsync<MotorcycleDto>();

        // Act
        var response = await _client.DeleteAsync($"/motos/{createdMotorcycle!.Identificador}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/motos/{createdMotorcycle.Identificador}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
