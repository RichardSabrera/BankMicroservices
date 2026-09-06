using System.Text.Json;
using ClientePersonaApi.Application.Services;
using ClientePersonaApi.Domain.Entities;
using ClientePersonaApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace ClientePersonaApi.Controllers;

[ApiController]
[Route("clientes")]
[Route("api/clientes")]
[ManejoExcepciones]
public class ClientesController : ControllerBase
{
    private readonly IClienteService _clienteService;

    public ClientesController(IClienteService clienteService)
    {
        _clienteService = clienteService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
    {
        var clientes = await _clienteService.GetAllAsync();
        return Ok(clientes);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Cliente>> GetCliente(int id)
    {
        var cliente = await _clienteService.GetByIdAsync(id);
        if (cliente == null)
        {
            return NotFound(new { mensaje = $"Cliente con ID {id} no encontrado" });
        }
        return Ok(cliente);
    }

    [HttpGet("clienteId/{clienteId}")]
    public async Task<ActionResult<Cliente>> GetByClienteId(string clienteId)
    {
        var cliente = await _clienteService.GetByClienteIdAsync(clienteId);
        if (cliente == null)
        {
            return NotFound(new { mensaje = $"Cliente con ClienteId {clienteId} no encontrado" });
        }
        return Ok(cliente);
    }

    [HttpPost]
    public async Task<ActionResult<Cliente>> CreateCliente([FromBody] Cliente cliente)
    {
        var created = await _clienteService.CreateAsync(cliente);
        return CreatedAtAction(nameof(GetCliente), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCliente(int id, [FromBody] Cliente cliente)
    {
        await _clienteService.UpdateAsync(id, cliente);
        return Ok(cliente);
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchCliente(int id, [FromBody] JsonElement patchData)
    {
        await _clienteService.PatchAsync(id, patchData);
        var updated = await _clienteService.GetByIdAsync(id);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCliente(int id)
    {
        await _clienteService.DeleteAsync(id);
        return Ok(new { mensaje = "Cliente eliminado correctamente" });
    }
}
