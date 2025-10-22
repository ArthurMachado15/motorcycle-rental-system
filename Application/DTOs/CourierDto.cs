using System.Text.Json.Serialization;

namespace Application.DTOs;

public class CourierDto
{
    [JsonPropertyName("identificador")]
    public string Identificador { get; set; } = string.Empty;
    
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;
    
    [JsonPropertyName("cnpj")]
    public string Cnpj { get; set; } = string.Empty;
    
    [JsonPropertyName("data_nascimento")]
    public DateTime DataNascimento { get; set; }
    
    [JsonPropertyName("numero_cnh")]
    public string NumeroCnh { get; set; } = string.Empty;
    
    [JsonPropertyName("tipo_cnh")]
    public string TipoCnh { get; set; } = string.Empty;
    
    [JsonPropertyName("imagem_cnh")]
    public string? ImagemCnh { get; set; }
}

public class CreateCourierDto
{
    [JsonPropertyName("identificador")]
    public string Identificador { get; set; } = string.Empty;
    
    [JsonPropertyName("nome")]
    public string Nome { get; set; } = string.Empty;
    
    [JsonPropertyName("cnpj")]
    public string Cnpj { get; set; } = string.Empty;
    
    [JsonPropertyName("data_nascimento")]
    public DateTime DataNascimento { get; set; }
    
    [JsonPropertyName("numero_cnh")]
    public string NumeroCnh { get; set; } = string.Empty;
    
    [JsonPropertyName("tipo_cnh")]
    public string TipoCnh { get; set; } = string.Empty;
    
    [JsonPropertyName("imagem_cnh")]
    public string? ImagemCnh { get; set; }
}

public class UploadCnhImageDto
{
    [JsonPropertyName("imagem_cnh")]
    public string ImagemCnh { get; set; } = string.Empty;
}
