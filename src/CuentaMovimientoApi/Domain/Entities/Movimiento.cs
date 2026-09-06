namespace CuentaMovimientoApi.Domain.Entities;

public class Movimiento
{
    public int Id { get; set; }
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string TipoMovimiento { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public decimal Saldo { get; set; }
    public string NumeroCuenta { get; set; } = string.Empty;
}
