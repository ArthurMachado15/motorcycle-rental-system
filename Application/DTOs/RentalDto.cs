using System.Text.Json.Serialization;

namespace Application.DTOs;

public class RentalDto
{
    [JsonPropertyName("identificador")]
    public string Identificador { get; set; } = string.Empty;
    
    [JsonPropertyName("valor_diaria")]
    public decimal ValorDiaria { get; set; }
    
    [JsonPropertyName("entregador_id")]
    public string EntregadorId { get; set; } = string.Empty;
    
    [JsonPropertyName("moto_id")]
    public string MotoId { get; set; } = string.Empty;
    
    [JsonPropertyName("data_inicio")]
    public DateTime DataInicio { get; set; }
    
    [JsonPropertyName("data_termino")]
    public DateTime DataTermino { get; set; }
    
    [JsonPropertyName("data_previsao_termino")]
    public DateTime DataPrevisaoTermino { get; set; }
    
    [JsonPropertyName("data_devolucao")]
    public DateTime? DataDevolucao { get; set; }
}

public class CreateRentalDto
{
    [JsonPropertyName("entregador_id")]
    public string EntregadorId { get; set; } = string.Empty;
    
    [JsonPropertyName("moto_id")]
    public string MotoId { get; set; } = string.Empty;
    
    [JsonPropertyName("data_inicio")]
    public DateTime DataInicio { get; set; }
    
    [JsonPropertyName("data_termino")]
    public DateTime DataTermino { get; set; }
    
    [JsonPropertyName("data_previsao_termino")]
    public DateTime DataPrevisaoTermino { get; set; }
    
    [JsonPropertyName("plano")]
    public int Plano { get; set; }
}

public class ReturnRentalDto
{
    [JsonPropertyName("data_devolucao")]
    public DateTime DataDevolucao { get; set; }
}
