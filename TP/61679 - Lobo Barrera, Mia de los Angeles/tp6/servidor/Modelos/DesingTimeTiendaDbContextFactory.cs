using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace servidor.Modelos  // Ajusta el namespace según tu proyecto
{
    public class DesignTimeTiendaDbContextFactory : IDesignTimeDbContextFactory<TiendaDbContext>
    {
        public TiendaDbContext CreateDbContext(string[] args)
        {
            // Configura el lector de configuración para cargar appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<TiendaDbContext>();
            optionsBuilder.UseSqlite(connectionString);

            return new TiendaDbContext(optionsBuilder.Options);
        }
    }
}
