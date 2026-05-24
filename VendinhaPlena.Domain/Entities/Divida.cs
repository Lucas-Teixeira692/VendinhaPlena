using System;

namespace VendinhaPlena.Domain.Entities
{
    public class Divida
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; } = null!;
        public decimal Valor { get; set; }
        public bool EstaPaga { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataPagamento { get; set; }
    }
}