using System.Text.Json.Serialization;

namespace Application.DTOs;

public class MotorcycleDto
{
    [JsonPropertyName("identificador")]
    public string Identificador { get; set; } = string.Empty;
    
    [JsonPropertyName("ano")]
    public int Ano { get; set; }
    
    [JsonPropertyName("modelo")]
    public string Modelo { get; set; } = string.Empty;
    
    [JsonPropertyName("placa")]
    public string Placa { get; set; } = string.Empty;
}

public class CreateMotorcycleDto
{
    [JsonPropertyName("identificador")]
    public string Identificador { get; set; } = string.Empty;
    
    [JsonPropertyName("ano")]
    public int Ano { get; set; }
    
    [JsonPropertyName("modelo")]
    public string Modelo { get; set; } = string.Empty;
    
    [JsonPropertyName("placa")]
    public string Placa { get; set; } = string.Empty;
}

public class UpdateMotorcycleLicensePlateDto
{
    [JsonPropertyName("placa")]
    public string Placa { get; set; } = string.Empty;
}

public class ErrorResponse
{
    [JsonPropertyName("mensagem")]
    public string Mensagem { get; set; } = string.Empty;
}
