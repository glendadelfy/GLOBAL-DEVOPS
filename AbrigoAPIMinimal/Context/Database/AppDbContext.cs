using AbrigoAPIMinimal.Model;
using Microsoft.EntityFrameworkCore;

namespace AbrigoAPIMinimal.Context.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Abrigo> Abrigos { get; set; }
        public DbSet<Evento> Eventos { get; set; } // Novo DbSet
        public DbSet<Pessoa> Pessoas { get; set; }
        public DbSet<Recurso> Recursos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

    }
}
