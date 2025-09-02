using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Application.Commands.Owners;
using RealEstate.Application.Commands.Properties;
using RealEstate.Application.Mappers;
using RealEstate.Application.Queries.Owners;
using RealEstate.Application.Queries.Properties;
using System.Reflection;

namespace RealEstate.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register mappers
        services.AddScoped<PropertyMapper>();
        services.AddScoped<OwnerMapper>();

        // Register command handlers
        services.AddScoped<CreatePropertyCommandHandler>();
        services.AddScoped<UpdatePropertyCommandHandler>();
        services.AddScoped<ChangePriceCommandHandler>();
        services.AddScoped<AddImageCommandHandler>();
        services.AddScoped<CreateOwnerCommandHandler>();

        // Register query handlers
        services.AddScoped<GetPropertyQueryHandler>();
        services.AddScoped<GetPropertiesQueryHandler>();
        services.AddScoped<GetOwnerQueryHandler>();

        return services;
    }
}