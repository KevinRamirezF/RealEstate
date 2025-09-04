using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RealEstate.Application.Common.Interfaces;
using RealEstate.Infrastructure.Configuration;
using RealEstate.Infrastructure.Data;
using RealEstate.Infrastructure.Identity;
using RealEstate.Infrastructure.Persistence;
using RealEstate.Infrastructure.Repositories;
using System;

namespace RealEstate.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Configure Database Settings with IOptions pattern
            services.Configure<DatabaseSettings>(configuration.GetSection(DatabaseSettings.SectionName));
            services.AddSingleton<IDbSettings>(provider => provider.GetRequiredService<IOptions<DatabaseSettings>>().Value);

            // Configure DbContext with enhanced options
            services.AddDbContext<RealEstateDbContext>((serviceProvider, options) =>
            {
                var dbSettings = serviceProvider.GetRequiredService<IDbSettings>();
                
                options.UseSqlServer(dbSettings.ConnectionString, sqlOptions =>
                {
                    sqlOptions.MigrationsAssembly(typeof(RealEstateDbContext).Assembly.FullName);
                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                    sqlOptions.CommandTimeout(dbSettings.CommandTimeout);
                    
                    if (dbSettings.EnableRetryOnFailure)
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: dbSettings.MaxRetryCount,
                            maxRetryDelay: dbSettings.MaxRetryDelay,
                            errorNumbersToAdd: null);
                    }
                });

                // Enable sensitive data logging in development only
                options.EnableSensitiveDataLogging(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");
                options.EnableDetailedErrors(Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development");
            });

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<RealEstateDbContext>()
                .AddDefaultTokenProviders();

            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IOwnerRepository, OwnerRepository>();

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Register Data Seeder
            services.AddScoped<DataSeeder>();

            return services;
        }
    }
}
