
using RealEstate.Application;
using RealEstate.API.Extensions;
using RealEstate.API.Filters;
using RealEstate.Infrastructure;
using RealEstate.Infrastructure.Data;
using Serilog;

namespace RealEstate.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure Serilog
            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

            // Add services to the container.
            builder.Services.AddApplicationServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            // Add Problem Details support
            builder.Services.AddProblemDetailsSupport();

            // Add Output Caching
            builder.Services.AddOutputCache(options =>
            {
                options.AddBasePolicy(policy => policy.Cache());
            });

            // Configure controllers with global filters
            builder.Services.AddControllers(options =>
            {
                // Add global validation filter
                options.Filters.Add<ValidationFilter>();
                options.Filters.Add<FluentValidationExceptionFilter>();
            });
            
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            
            // Add Serilog request logging
            app.UseSerilogRequestLogging();
            
            // Add global exception handling (must be first in pipeline)
            app.UseGlobalExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseOutputCache();

            app.UseAuthorization();

            app.MapControllers();

            // Seed data in development
            if (app.Environment.IsDevelopment())
            {
                using var scope = app.Services.CreateScope();
                var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                await seeder.SeedAsync();
            }

            app.Run();
        }
    }
}
