using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace VendinhaPlena.Domain.Entities
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do cliente é obrigatório", AllowEmptyStrings = false)]
        [StringLength(80, MinimumLength = 3, ErrorMessage = "O nome deve ter de 3 a 80 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório", AllowEmptyStrings = false)]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF inválido. Deve conter 11 dígitos numéricos.")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        public DateTime DataNascimento { get; set; }

        [EmailAddress(ErrorMessage = "E-mail inválido.")]
        public string? Email { get; set; }

        public ICollection<Divida> Dividas { get; set; } = new List<Divida>();

    
        [Range(18, 99, ErrorMessage = "O cliente deve ter no mínimo 18 anos de idade.")]
        public int Idade
        {
            get
            {
                var hoje = DateTime.Today;
                var totalAnos = hoje.Year - DataNascimento.Year;
                var diaAnoNascimento = hoje.AddYears(-totalAnos);

                if (DataNascimento > diaAnoNascimento)
                {
                    totalAnos--;
                }
                return totalAnos;
            }
        }

        public decimal TotalDividas => Dividas.Where(d => !d.EstaPaga).Sum(d => d.Valor);
    }
}