using System.Text.Json.Serialization;

namespace CuentaMovimientoApi.DTOs;

public class ReporteDto
{
    [JsonPropertyName("Fecha")]
    public string Fecha { get; set; } = string.Empty;

    [JsonPropertyName("Cliente")]
    public string Cliente { get; set; } = string.Empty;

    [JsonPropertyName("Numero Cuenta")]
    public string NumeroCuenta { get; set; } = string.Empty;

    [JsonPropertyName("Tipo")]
    public string Tipo { get; set; } = string.Empty;

    [JsonPropertyName("Saldo Inicial")]
    public decimal SaldoInicial { get; set; }

    [JsonPropertyName("Estado")]
    public bool Estado { get; set; }

    [JsonPropertyName("Movimiento")]
    public decimal Movimiento { get; set; }

    [JsonPropertyName("Saldo Disponible")]
    public decimal SaldoDisponible { get; set; }
}
