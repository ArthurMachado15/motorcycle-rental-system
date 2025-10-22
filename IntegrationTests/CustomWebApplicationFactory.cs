using Application.Common.Interfaces;
using Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly string _databaseName;

    public CustomWebApplicationFactory(string databaseName = "TestDb")
    {
        _databaseName = databaseName;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove o DbContext existente
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Remove o IMessagePublisher real
            var publisherDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IMessagePublisher));
            if (publisherDescriptor != null)
                services.Remove(publisherDescriptor);

            // Adiciona DbContext com InMemory Database ÚNICO por teste
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
            });

            // Mock do IMessagePublisher para evitar dependência do RabbitMQ
            services.AddSingleton<IMessagePublisher>(sp =>
            {
                var mock = new Moq.Mock<IMessagePublisher>();
                mock.Setup(x => x.PublishAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);
                return mock.Object;
            });

            // Build the service provider e inicializa o banco
            var sp = services.BuildServiceProvider();

            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var db = scopedServices.GetRequiredService<ApplicationDbContext>();
            
            // Garante que o banco está criado
            db.Database.EnsureCreated();
        });

        builder.UseEnvironment("Testing");
    }
}
