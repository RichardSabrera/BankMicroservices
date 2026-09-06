namespace CuentaMovimientoApi.Domain.Entities;

public class ClienteReadModel
{
    public int Id { get; set; }
    public string ClienteId { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Identificacion { get; set; } = string.Empty;
    public bool Estado { get; set; } = true;
}
