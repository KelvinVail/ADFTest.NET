using ADFTest.Net.Source.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ADFTest.Net.ConsoleApp
{
    public class SourceDbContextFactory : IDesignTimeDbContextFactory<SourceContext>
    {
        public SourceContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json", false)
                .Build();

            var connStr = configuration.GetConnectionString("SourceConnectionString");

            var optionsBuilder = new DbContextOptionsBuilder<SourceContext>();
            optionsBuilder.UseSqlServer(connStr, o => o.EnableRetryOnFailure());

            return new SourceContext(optionsBuilder.Options);
        }
    }
}
