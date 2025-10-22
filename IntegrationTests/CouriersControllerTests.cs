using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;

namespace IntegrationTests;

public class CouriersControllerTests : IntegrationTestBase
{
    public CouriersControllerTests()
    {
    }

    [Fact]
    public async Task CreateCourier_ValidData_ReturnsCreated()
    {
        // Arrange
        var createDto = new CreateCourierDto
        {
            Identificador = "entregador-test-1",
            Nome = "John Doe",
            Cnpj = "12345678901234",
            DataNascimento = new DateTime(1990, 5, 15),
            NumeroCnh = "ABC123456789",
            TipoCnh = "A"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/entregadores", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CourierDto>();
        result.Should().NotBeNull();
        result!.Nome.Should().Be(createDto.Nome);
        result.Cnpj.Should().Be(createDto.Cnpj);
        result.TipoCnh.Should().Be(createDto.TipoCnh);
    }

    [Theory]
    [InlineData("A")]
    [InlineData("B")]
    [InlineData("A+B")]
    [InlineData("AB")]
    public async Task CreateCourier_ValidLicenseTypes_ReturnsCreated(string licenseType)
    {
        // Arrange
        var createDto = new CreateCourierDto
        {
            Identificador = $"entregador-{licenseType}-{Guid.NewGuid():N}"[..20],
            Nome = $"Test Courier {licenseType}",
            Cnpj = $"111111{licenseType.GetHashCode():D8}",
            DataNascimento = new DateTime(1985, 3, 20),
            NumeroCnh = $"LIC{licenseType}{Guid.NewGuid():N}"[..20],
            TipoCnh = licenseType
        };

        // Act
        var response = await _client.PostAsJsonAsync("/entregadores", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateCourier_InvalidLicenseType_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateCourierDto
        {
            Identificador = "entregador-invalid",
            Nome = "Jane Doe",
            Cnpj = "98765432109876",
            DataNascimento = new DateTime(1992, 8, 10),
            NumeroCnh = "XYZ987654321",
            TipoCnh = "C" // Invalid type
        };

        // Act
        var response = await _client.PostAsJsonAsync("/entregadores", createDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCourier_DuplicateCNPJ_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateCourierDto
        {
            Identificador = "entregador-dup-cnpj-1",
            Nome = "Duplicate CNPJ Test",
            Cnpj = "11111111111111",
            DataNascimento = new DateTime(1988, 12, 5),
            NumeroCnh = "DUP123456789",
            TipoCnh = "A+B"
        };

        // Create first courier
        await _client.PostAsJsonAsync("/entregadores", createDto);

        // Act - Try to create with same CNPJ
        var createDto2 = new CreateCourierDto
        {
            Identificador = "entregador-dup-cnpj-2",
            Nome = "Another Person",
            Cnpj = createDto.Cnpj, // Same CNPJ
            DataNascimento = new DateTime(1990, 1, 1),
            NumeroCnh = "DIFF123456789",
            TipoCnh = "B"
        };
        var response = await _client.PostAsJsonAsync("/entregadores", createDto2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateCourier_DuplicateDriverLicenseNumber_ReturnsBadRequest()
    {
        // Arrange
        var createDto = new CreateCourierDto
        {
            Identificador = "entregador-dup-cnh-1",
            Nome = "Duplicate License Test",
            Cnpj = "22222222222222",
            DataNascimento = new DateTime(1991, 7, 20),
            NumeroCnh = "LIC999999999",
            TipoCnh = "A"
        };

        // Create first courier
        await _client.PostAsJsonAsync("/entregadores", createDto);

        // Act - Try to create with same license number
        var createDto2 = new CreateCourierDto
        {
            Identificador = "entregador-dup-cnh-2",
            Nome = "Different Person",
            Cnpj = "33333333333333",
            DataNascimento = new DateTime(1989, 3, 15),
            NumeroCnh = createDto.NumeroCnh, // Same license number
            TipoCnh = "B"
        };
        var response = await _client.PostAsJsonAsync("/entregadores", createDto2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
