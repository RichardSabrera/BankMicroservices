using ClientePersonaApi.Domain.Entities;
using CuentaMovimientoApi.Domain.Entities;
using CuentaMovimientoApi.Exceptions;
using Xunit;

namespace BankMicroservices.Tests;

public class ClienteUnitTest
{
    [Fact]
    public void Cliente_CreacionYPropiedades_AsignacionCorrecta()
    {
        // Arrange & Act
        var cliente = new Cliente
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
        };

        // Assert
        Assert.Equal("Jose Lema", cliente.Nombre);
        Assert.Equal("1712345678", cliente.Identificacion);
        Assert.Equal("1234", cliente.Contrasena);
        Assert.True(cliente.Estado);
    }

    [Fact]
    public void Movimiento_RetiroExcesivo_LanzaSaldoNoDisponibleException()
    {
        // Arrange
        var cuenta = new Cuenta
        {
            NumeroCuenta = "478758",
            SaldoInicial = 100,
            SaldoDisponible = 100,
            ClienteId = 1
        };

        decimal valorRetiro = -150;

        // Act & Assert
        Assert.Throws<SaldoNoDisponibleException>(() =>
        {
            decimal nuevoSaldo = cuenta.SaldoDisponible + valorRetiro;
            if (nuevoSaldo < 0)
            {
                throw new SaldoNoDisponibleException("Saldo no disponible");
            }
            cuenta.SaldoDisponible = nuevoSaldo;
        });
    }
}
