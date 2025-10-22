using System.Text;
using System.Text.Json;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Infrastructure.Messaging;

public class MotorcycleRegisteredConsumer : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<MotorcycleRegisteredConsumer> _logger;
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;

    public MotorcycleRegisteredConsumer(
        IServiceProvider serviceProvider,
        ILogger<MotorcycleRegisteredConsumer> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQ:Host"] ?? "localhost",
            Port = int.Parse(_configuration["RabbitMQ:Port"] ?? "5672"),
            UserName = _configuration["RabbitMQ:Username"] ?? "guest",
            Password = _configuration["RabbitMQ:Password"] ?? "guest"
        };

        try
        {
            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            await _channel.QueueDeclareAsync(
                queue: "motorcycle-registered",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var motorcycleEvent = JsonSerializer.Deserialize<MotorcycleRegisteredEvent>(message);

                    if (motorcycleEvent != null && motorcycleEvent.Year == 2024)
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var eventLogRepository = scope.ServiceProvider.GetRequiredService<IMotorcycleEventLogRepository>();
                        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                        var eventLog = new MotorcycleEventLog
                        {
                            Id = Guid.NewGuid(),
                            MotorcycleId = motorcycleEvent.MotorcycleId,
                            Year = motorcycleEvent.Year,
                            Model = motorcycleEvent.Model,
                            LicensePlate = motorcycleEvent.LicensePlate,
                            RegisteredAt = motorcycleEvent.RegisteredAt
                        };

                        await eventLogRepository.AddAsync(eventLog, stoppingToken);
                        await unitOfWork.SaveChangesAsync(stoppingToken);

                        _logger.LogInformation("Motorcycle event logged for year 2024: {LicensePlate}", motorcycleEvent.LicensePlate);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing motorcycle registered event");
                    await _channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
                }
            };

            await _channel.BasicConsumeAsync(
                queue: "motorcycle-registered",
                autoAck: false,
                consumer: consumer,
                cancellationToken: stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in MotorcycleRegisteredConsumer");
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }

    private class MotorcycleRegisteredEvent
    {
        public Guid MotorcycleId { get; set; }
        public int Year { get; set; }
        public string Model { get; set; } = string.Empty;
        public string LicensePlate { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
    }
}
