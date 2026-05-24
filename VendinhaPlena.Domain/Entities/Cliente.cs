using System;
using System.Collections.Generic;
using System.Linq;

namespace VendinhaPlena.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string? Email { get; set; }
        
        public ICollection<Divida> Dividas { get; set; } = new List<Divida>();


        public int Idade 
        {
            get
            {
                var hoje = DateTime.Today;
                var idade = hoje.Year - DataNascimento.Year;
                if (DataNascimento.Date > hoje.AddYears(-idade)) idade--;
                return idade;
            }
        }

        public decimal TotalDividas => Dividas.Where(d => !d.EstaPaga).Sum(d => d.Valor);
    }
}