using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace CarteirinhaVacinacao.Models
{
    public class VacinacaoContext : DbContext
    {
        public VacinacaoContext(DbContextOptions<VacinacaoContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pessoa>()
                   .HasKey(p => p.IdPessoa);
            modelBuilder.Entity<Vacina>()
                .HasKey(p => p.IdVacina);
            modelBuilder.Entity<PessoaVacinada>()
                .HasKey(p => p.IdPessoaVacinada);

            modelBuilder.Entity<Pessoa>().ToTable("Pessoa");
            modelBuilder.Entity<Vacina>().ToTable("Vacina");
            modelBuilder.Entity<PessoaVacinada>().ToTable("PessoaVacinada");
        }

        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Vacina> Vacinas { get; set; }
        public DbSet<PessoaVacinada> PessoasVacinadas { get; set; }        

    }
}
