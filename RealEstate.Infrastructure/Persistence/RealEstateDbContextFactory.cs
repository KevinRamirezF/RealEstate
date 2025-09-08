using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace RealEstate.Infrastructure.Persistence
{
    public class RealEstateDbContextFactory : IDesignTimeDbContextFactory<RealEstateDbContext>
    {
        public RealEstateDbContext CreateDbContext(string[] args)
        {
            // This factory is used by EF Core tools. It requires a path to the appsettings.json in the API project.
            var basePath = Directory.GetCurrentDirectory().Replace("RealEstate.Infrastructure", "RealEstate.API");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<RealEstateDbContext>();
            var connectionString = configuration.GetSection("DatabaseSettings:ConnectionString").Value;
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string not found in DatabaseSettings:ConnectionString");
            }

            builder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(RealEstateDbContext).Assembly.FullName);
                sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
            });

            return new RealEstateDbContext(builder.Options);
        }
    }
}
