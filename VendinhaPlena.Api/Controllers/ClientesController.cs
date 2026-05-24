using Microsoft.AspNetCore.Mvc;
using VendinhaPlena.Application.Services;
using VendinhaPlena.Domain.Entities;

namespace VendinhaPlena.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly ClienteService _clienteService;

        public ClientesController(ClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] Cliente cliente)
        {
            try { return Ok(await _clienteService.CriarClienteAsync(cliente)); }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet]
        public async Task<IActionResult> Listar([FromQuery] string? busca, [FromQuery] int pagina = 1)
        {
        
            var clientes = await _clienteService.ObterClientesPaginadosAsync(busca, pagina);
            return Ok(clientes);
        }

        [HttpPost("{id}/dividas")]
        public async Task<IActionResult> AdicionarDivida(int id, [FromBody] decimal valor)
        {
            try 
            { 
                await _clienteService.AdicionarDividaAsync(id, valor); 
                return Ok(); 
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpPatch("dividas/{dividaId}/pagar")]
        public async Task<IActionResult> PagarDivida(int dividaId)
        {
            await _clienteService.MarcarDividaComoPagaAsync(dividaId);
            return Ok();
        }
    }
}