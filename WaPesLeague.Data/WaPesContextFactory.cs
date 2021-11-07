using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace WaPesLeague.Data
{
    public class WaPesContextFactory : IDesignTimeDbContextFactory<WaPesDbContext>
    {
        public WaPesDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.local.json")
                .Build();

            var dbContextBuilder = new DbContextOptionsBuilder<WaPesDbContext>();

            var connectionString = configuration.GetConnectionString("WaPesLeagueDb");
            dbContextBuilder.UseSqlServer(connectionString);

            return new WaPesDbContext(dbContextBuilder.Options);
        }
    }
}
