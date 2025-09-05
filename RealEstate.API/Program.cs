
using RealEstate.Application;
using RealEstate.API.Extensions;
using RealEstate.API.Filters;
using RealEstate.Infrastructure;
using RealEstate.Infrastructure.Data;
using Serilog;
using Scalar.AspNetCore;

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
            
            // Configure OpenAPI for Scalar
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { 
                    Title = "Million Realty API", 
                    Version = "v1.0.0",
                    Description = @"**Million Realty LLC - Real Estate Management API**

                    A comprehensive Real Estate management API built for luxury real estate operations in South Florida.

                    **Company Information:**
                    - **Legal Name:** Million Realty LLC
                    - **Brand:** MILLION  
                    - **Address:** 237 S Dixie Hwy, 4th Floor, Suite 465, Coral Gables, FL 33133
                    - **Licensed In:** Florida
                    - **Recognition:** #1 Top Team - New construction sales in the U.S. ($2.1B+ in sales)
                    - **Specialization:** Luxury real estate in South Florida
                    - **Website:** https://www.millionluxury.com

                    **API Features:**
                    - Full CRUD operations for properties and owners
                    - Advanced filtering and search capabilities  
                    - Partial updates via PATCH operations
                    - Real-time data validation
                    - Optimistic concurrency control
                    - Comprehensive audit trails

                    **Developed By:** Ing. Kevin Guillermo Ramirez  
                    **Role:** Senior Backend Developer .NET  
                    **Purpose:** Technical test for Million Realty Senior .NET developer interview",
                    Contact = new() {
                        Name = "Ing. Kevin Guillermo Ramirez",
                        Email = "kevinramirezf@outlook.com"
                    }
                });
                
                // Include XML comments for better documentation
                var xmlPath = Path.Combine(AppContext.BaseDirectory, "RealEstate.API.xml");
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
                
                var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, "RealEstate.Application.xml");
                if (File.Exists(applicationXmlPath))
                {
                    c.IncludeXmlComments(applicationXmlPath);
                }
                
                // Add more detailed info
                c.EnableAnnotations();
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            
            // Add Serilog request logging
            app.UseSerilogRequestLogging();
            
            // Add global exception handling (must be first in pipeline)
            app.UseGlobalExceptionHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.MapScalarApiReference(options =>
                {
                    options.Title = "Million Realty API Documentation";
                    options.Theme = ScalarTheme.Default;
                    options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
                    options.OpenApiRoutePattern = "/swagger/{documentName}/swagger.json";
                });
                
                // Redirect root to Scalar documentation
                app.MapGet("/", () => Results.Redirect("/scalar"));
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
