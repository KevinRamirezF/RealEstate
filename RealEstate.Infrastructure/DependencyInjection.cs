using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Application.Common.Interfaces;
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
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<RealEstateDbContext>(options =>
                options.UseSqlServer(connectionString,
                    builder => builder.MigrationsAssembly(typeof(RealEstateDbContext).Assembly.FullName)
                                      .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<RealEstateDbContext>()
                .AddDefaultTokenProviders();

            // Register repositories
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IPropertyRepository, PropertyRepository>();
            services.AddScoped<IOwnerRepository, OwnerRepository>();

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
