namespace CuentaMovimientoApi.Domain.Entities;

public class Cuenta
{
    public int Id { get; set; }
    public string NumeroCuenta { get; set; } = string.Empty;
    public string TipoCuenta { get; set; } = string.Empty;
    public decimal SaldoInicial { get; set; }
    public decimal SaldoDisponible { get; set; }
    public bool Estado { get; set; } = true;
    public int ClienteId { get; set; }
}
