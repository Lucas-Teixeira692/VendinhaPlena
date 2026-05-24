using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using VendinhaPlena.Domain.Entities;
using VendinhaPlena.Infrastructure.Data;

namespace VendinhaPlena.Application.Services
{
    public class ClienteService
    {
        private readonly VendinhaDbContext _context;

        public ClienteService(VendinhaDbContext context)
        {
            _context = context;
        }

        public async Task<Cliente> CriarClienteAsync(Cliente cliente)
        {
            // Validar CPF básico
            if (!Regex.IsMatch(cliente.Cpf, @"^\d{11}$")) 
                throw new ArgumentException("CPF inválido."); 

            if (await _context.Clientes.AnyAsync(c => c.Cpf == cliente.Cpf))
                throw new InvalidOperationException("CPF já cadastrado."); 

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente; 
        }

        public async Task<object> ObterClientesPaginadosAsync(string? buscaNome, int pagina)
        {
            var query = _context.Clientes.Include(c => c.Dividas).AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscaNome))
                query = query.Where(c => c.Nome.Contains(buscaNome)); 

           
            var clientesOrdenados = await query.ToListAsync();
            var clientes = clientesOrdenados
                .OrderByDescending(c => c.TotalDividas)
                .Skip((pagina - 1) * 10)
                .Take(10) 
                .Select(c => new { c.Id, c.Nome, c.Cpf, c.Idade, c.TotalDividas }) 
                .ToList();

            return clientes;
        }
        
        public async Task AdicionarDividaAsync(int clienteId, decimal valor)
        {
            var cliente = await _context.Clientes.Include(c => c.Dividas).FirstOrDefaultAsync(c => c.Id == clienteId);
            if (cliente == null) throw new Exception("Cliente não encontrado.");

            
            if (cliente.Dividas.Any(d => !d.EstaPaga))
                throw new InvalidOperationException("Cliente já possui uma dívida em aberto."); 

            var novaDivida = new Divida { ClienteId = clienteId, Valor = valor, EstaPaga = false };
            _context.Dividas.Add(novaDivida); 
            await _context.SaveChangesAsync();
        }
        
        public async Task MarcarDividaComoPagaAsync(int dividaId)
        {
            var divida = await _context.Dividas.FindAsync(dividaId);
            if (divida == null) throw new Exception("Dívida não encontrada.");
            
            divida.EstaPaga = true; 
            divida.DataPagamento = DateTime.UtcNow; 
            await _context.SaveChangesAsync(); 
        }
    }
}