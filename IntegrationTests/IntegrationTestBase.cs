namespace IntegrationTests;

public abstract class IntegrationTestBase : IDisposable
{
    protected readonly HttpClient _client;
    protected readonly CustomWebApplicationFactory<Program> _factory;

    protected IntegrationTestBase()
    {
        // Cria um banco InMemory ÚNICO para cada instância de teste
        var databaseName = $"TestDb_{Guid.NewGuid()}";
        _factory = new CustomWebApplicationFactory<Program>(databaseName);
        _client = _factory.CreateClient();
    }

    public void Dispose()
    {
        _client?.Dispose();
        _factory?.Dispose();
    }
}
