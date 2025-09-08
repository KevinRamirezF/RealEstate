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
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        // Register mappers
        services.AddSingleton<PropertyMapper>();
        services.AddSingleton<OwnerMapper>();

        // Register command handlers
        services.AddScoped<CreatePropertyCommandHandler>();
        services.AddScoped<UpdatePropertyCommandHandler>();
        services.AddScoped<PatchPropertyCommandHandler>();
        services.AddScoped<DeletePropertyCommandHandler>();
        services.AddScoped<ChangePriceCommandHandler>();
        services.AddScoped<AddImageCommandHandler>();
        services.AddScoped<CreateOwnerCommandHandler>();
        services.AddScoped<UpdateOwnerCommandHandler>();
        services.AddScoped<PatchOwnerCommandHandler>();
        services.AddScoped<DeleteOwnerCommandHandler>();

        // Register query handlers
        services.AddScoped<GetPropertiesQueryHandler>();
        services.AddScoped<GetPropertyByIdQueryHandler>();
        services.AddScoped<GetOwnersQueryHandler>();
        services.AddScoped<GetOwnerByIdQueryHandler>();

        return services;
    }
}