using ClientePersonaApi.Domain.Entities;
using ClientePersonaApi.Infrastructure.Persistence;

namespace ClientePersonaApi.Data;

public static class ClienteSeedData
{
    public static void Seed(ClienteDbContext context)
    {
        if (!context.Clientes.Any())
        {
            context.Clientes.AddRange(
                new Cliente
                {
                    Nombre = "Jose Lema",
                    Genero = "Masculino",
                    Edad = 35,
                    Identificacion = "1712345678",
                    Direccion = "Otavalo sn y principal",
                    Telefono = "098254785",
                    ClienteId = "CLI-A8F93B12C4E5",
                    Contrasena = "1234",
                    Estado = true
                },
                new Cliente
                {
                    Nombre = "Marianela Montalvo",
                    Genero = "Femenino",
                    Edad = 30,
                    Identificacion = "1798765432",
                    Direccion = "Amazonas y NNUU",
                    Telefono = "097548965",
                    ClienteId = "CLI-B9E02A14D5F6",
                    Contrasena = "5678",
                    Estado = true
                },
                new Cliente
                {
                    Nombre = "Juan Osorio",
                    Genero = "Masculino",
                    Edad = 40,
                    Identificacion = "1755554444",
                    Direccion = "13 junio y Equinoccial",
                    Telefono = "098874587",
                    ClienteId = "CLI-C1D2E3F4A5B6",
                    Contrasena = "1245",
                    Estado = true
                }
            );

            context.SaveChanges();
        }
    }
}
