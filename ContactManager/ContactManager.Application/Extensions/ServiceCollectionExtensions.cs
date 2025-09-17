using ContactManager.Application.Interfaces;
using ContactManager.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ContactManager.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IContactService, ContactService>();
        services.AddScoped<ICsvService, CsvService>();

        return services;
    }
}