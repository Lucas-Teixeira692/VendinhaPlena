using Microsoft.EntityFrameworkCore;
using VendinhaPlena.Domain.Entities;

namespace VendinhaPlena.Infrastructure.Data
{
    public class VendinhaDbContext : DbContext
    {
        public VendinhaDbContext(DbContextOptions<VendinhaDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Divida> Dividas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Cpf)
                .IsUnique();

            modelBuilder.Entity<Cliente>()
                .HasMany(c => c.Dividas)
                .WithOne(d => d.Cliente)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}