namespace CuentaMovimientoApi.Exceptions;

public class SaldoNoDisponibleException : Exception
{
    public SaldoNoDisponibleException() : base("Saldo no disponible")
    {
    }

    public SaldoNoDisponibleException(string message) : base(message)
    {
    }
}
