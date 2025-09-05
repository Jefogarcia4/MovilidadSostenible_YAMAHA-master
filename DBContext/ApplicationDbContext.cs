using Microsoft.EntityFrameworkCore;
using MovilidadSostenible_YAMAHA.Models;

namespace MovilidadSostenible_YAMAHA.DBContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
        {

        }
        
        public DbSet<EncuestasE> encuestas { get; set; }
        public DbSet<TrayectosE> trayectos { get; set; }
        public DbSet<TransportePublicoE> transportePublico { get; set; }
    }
}
