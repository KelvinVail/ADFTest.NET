using ADFTest.Net.Destination.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ADFTest.Net.ConsoleApp
{
    public class DestinationDbContextFactory : IDesignTimeDbContextFactory<DestinationContext>
    {
        public DestinationContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", false)
                .Build();

            var connStr = configuration.GetConnectionString("DestinationConnectionString");

            var optionsBuilder = new DbContextOptionsBuilder<DestinationContext>();
            optionsBuilder.UseSqlServer(connStr, o => o.EnableRetryOnFailure());

            return new DestinationContext(optionsBuilder.Options);
        }
    }
}
