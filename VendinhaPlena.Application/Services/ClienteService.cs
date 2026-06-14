using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using VendinhaPlena.Domain.Entities;
using VendinhaPlena.Infrastructure.Data;
using System.ComponentModel.DataAnnotations;

namespace VendinhaPlena.Application.Services
{
    public class ClienteService
    {
        private readonly VendinhaDbContext _context;

        public ClienteService(VendinhaDbContext context)
        {
            _context = context;
        }

        public bool CriarCliente(Cliente cliente, out List<ValidationResult> listaErros)
        {
            if (Validar(cliente, out listaErros) == false)
            {
                return false;
            }

            _context.Clientes.Add(cliente);
            _context.SaveChanges();
            return true;
        }

        public bool Validar(Cliente c, out List<ValidationResult> listaErros)
        {
            var contexto = new ValidationContext(c);
            var erros = new List<ValidationResult>();
            listaErros = erros;

            var objetoValido = Validator.TryValidateObject(c, contexto, erros, true);

            
            var existente = _context.Clientes.Any(item => item.Cpf == c.Cpf && item.Id != c.Id);
            if (existente)
            {
                erros.Add(new ValidationResult("CPF já cadastrado.", new[] { "Cpf" }));
                objetoValido = false;
            }

            if (!objetoValido)
            {
                foreach (var erro in erros)
                {
                    Console.WriteLine("{0}: {1}", erro.MemberNames.First(), erro.ErrorMessage);
                }
            }
            return objetoValido;
        }

        public List<Cliente> ObterClientesPaginados(string buscaNome, int pagina)
        {
            
            var query = _context.Clientes.Include(c => c.Dividas).AsQueryable();

            if (!string.IsNullOrWhiteSpace(buscaNome))
            {
                query = query.Where(c => c.Nome.Contains(buscaNome));
            }

            
            return query.ToList()
                .OrderByDescending(c => c.TotalDividas)
                .Skip((pagina - 1) * 10)
                .Take(10)
                .ToList();
        }

        public bool AtualizarCliente(int id, Cliente dadosAtualizados, out List<ValidationResult> listaErros)
        {
            dadosAtualizados.Id = id;
            if (Validar(dadosAtualizados, out listaErros) == false)
            {
                return false;
            }

            var cliente = _context.Clientes.Find(id);
            if (cliente == null) return false;

            cliente.Nome = dadosAtualizados.Nome;
            cliente.Email = dadosAtualizados.Email;
            cliente.DataNascimento = dadosAtualizados.DataNascimento;
            cliente.Cpf = dadosAtualizados.Cpf;

            _context.SaveChanges();
            return true;
        }

        public void ExcluirCliente(int id)
        {
            var cliente = _context.Clientes.Find(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                _context.SaveChanges();
            }
        }

        public List<Divida> ObterDividasPorCliente(int clienteId)
        {
            
            return _context.Dividas
                .Where(d => d.ClienteId == clienteId)
                .OrderByDescending(d => d.DataCriacao)
                .ToList();
        }

        public bool AdicionarDivida(int clienteId, decimal valor, out List<ValidationResult> listaErros)
        {
            var erros = new List<ValidationResult>();
            listaErros = erros;

            
            var possuiDividaAtiva = _context.Dividas.Any(d => d.ClienteId == clienteId && !d.EstaPaga);
            if (possuiDividaAtiva)
            {
                erros.Add(new ValidationResult("Cliente já possui uma dívida em aberto.", new[] { "ClienteId" }));
                return false;
            }

            var novaDivida = new Divida { ClienteId = clienteId, Valor = valor, EstaPaga = false, DataCriacao = DateTime.UtcNow };
            _context.Dividas.Add(novaDivida);
            _context.SaveChanges();
            return true;
        }

        public bool MarcarDividaComoPaga(int dividaId)
        {
            var divida = _context.Dividas.Find(dividaId);
            if (divida == null) return false;

            divida.EstaPaga = true;
            divida.DataPagamento = DateTime.UtcNow;
            _context.SaveChanges();
            return true;
        }
    }
}