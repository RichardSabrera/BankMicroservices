namespace ClientePersonaApi.Domain.Entities;

public class Cliente : Persona
{
    public string ClienteId { get; set; } = string.Empty;
    public string Contrasena { get; set; } = string.Empty;
    public bool Estado { get; set; } = true;
}
