using Application.Common.Interfaces;
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Messaging;
using Infrastructure.Repositories;
using Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
        services.AddScoped<ICourierRepository, CourierRepository>();
        services.AddScoped<IRentalRepository, RentalRepository>();
        services.AddScoped<IMotorcycleEventLogRepository, MotorcycleEventLogRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Messaging
        services.AddSingleton<IMessagePublisher, RabbitMqPublisher>();
        services.AddHostedService<MotorcycleRegisteredConsumer>();

        // Storage
        services.AddScoped<IFileStorage, LocalFileStorage>();

        return services;
    }
}
