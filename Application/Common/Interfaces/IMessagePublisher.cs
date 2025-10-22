namespace Application.Common.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync<T>(string queueName, T message, CancellationToken cancellationToken = default) where T : class;
}
