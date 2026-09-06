using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CuentaMovimientoApi.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BankMicroservices.Tests;

public class MovimientoIntegrationTest : IClassFixture<WebApplicationFactory<CuentaMovimientoApi.Program>>
{
    private readonly WebApplicationFactory<CuentaMovimientoApi.Program> _factory;

    public MovimientoIntegrationTest(WebApplicationFactory<CuentaMovimientoApi.Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
        });
    }

    [Fact]
    public async Task PostMovimiento_SaldoInsuficiente_RetornaBadRequestConMensajeSaldoNoDisponible()
    {
        // Arrange
        var client = _factory.CreateClient();

        var nuevoMovimiento = new Movimiento
        {
            NumeroCuenta = "225487",
            TipoMovimiento = "Retiro de 9000",
            Valor = -9000
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/movimientos", nuevoMovimiento);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        using var jsonDoc = JsonDocument.Parse(content);
        var root = jsonDoc.RootElement;

        Assert.True(root.TryGetProperty("mensaje", out var mensajeProp));
        Assert.Equal("Saldo no disponible", mensajeProp.GetString());
    }
}
