using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        public IActionResult Criar([FromBody] Cliente cliente)
        {
            if (_clienteService.CriarCliente(cliente, out List<ValidationResult> erros))
            {
                return Ok(cliente);
            }

            return BadRequest(erros.Select(e => e.ErrorMessage));
        }

        [HttpGet]
        public IActionResult Listar([FromQuery] string? busca, [FromQuery] int pagina = 1)
        {
            var clientes = _clienteService.ObterClientesPaginados(busca ?? string.Empty, pagina);
            return Ok(clientes);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, [FromBody] Cliente cliente)
        {
            if (_clienteService.AtualizarCliente(id, cliente, out List<ValidationResult> erros))
            {
                return Ok();
            }
            return BadRequest(erros.Select(e => e.ErrorMessage));
        }

        [HttpDelete("{id}")]
        public IActionResult Excluir(int id)
        {
            _clienteService.ExcluirCliente(id);
            return Ok();
        }

        [HttpGet("{id}/dividas")]
        public IActionResult ListarDividas(int id)
        {
            var dividas = _clienteService.ObterDividasPorCliente(id);
            return Ok(dividas);
        }

        [HttpPost("{id}/dividas")]
        public IActionResult AdicionarDivida(int id, [FromBody] decimal valor)
        {
            if (_clienteService.AdicionarDivida(id, valor, out List<ValidationResult> erros))
            {
                return Ok();
            }
            return BadRequest(erros.Select(e => e.ErrorMessage));
        }

        [HttpPatch("dividas/{dividaId}/pagar")]
        public IActionResult PagarDivida(int dividaId)
        {
            var sucesso = _clienteService.MarcarDividaComoPaga(dividaId);

            if (sucesso)
            {
                return Ok();
            }

            return BadRequest("Dívida não encontrada.");
        }
    }
}