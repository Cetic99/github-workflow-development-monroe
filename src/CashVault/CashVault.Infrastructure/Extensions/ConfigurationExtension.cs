using System.Text.Json;
using CashVault.Domain.Aggregates.DeviceAggregate.Configuration;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;

namespace CashVault.Infrastructure.Extensions;


public class DatabaseConfigurationSource : IConfigurationSource
{
    private readonly string _connectionString;

    public DatabaseConfigurationSource(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new DatabaseConfigurationProvider(_connectionString);
    }
}

public class DatabaseConfigurationProvider : ConfigurationProvider
{
    private readonly string _connectionString;

    public DatabaseConfigurationProvider(string connectionString)
    {
        _connectionString = connectionString;
    }

    public override void Load()
    {
        using var connection = new FbConnection(_connectionString);
        connection.Open();
        string query = @"SELECT ""Value"" FROM ""Configuration"" where ""Key"" = 'ServerConfiguration'";

        using var command = new FbCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {

            string serverConfigString = reader.GetString(0);

            if (string.IsNullOrEmpty(serverConfigString))
            {
                Data.Add("ServerConfiguration", JsonSerializer.Serialize(new ServerConfiguration()));
                continue;
            }

            var serverConfig = JsonSerializer.Deserialize<ServerConfiguration>(serverConfigString);

            if (serverConfig == null)
            {
                Data.Add("ServerConfiguration", JsonSerializer.Serialize(new ServerConfiguration()));
                continue;
            }

            Data.Add("ServerConfiguration", serverConfigString);
        }

        Data.TryGetValue("ServerConfiguration", out var configExists);

        if (configExists == null)
        {
            Data.Add("ServerConfiguration", JsonSerializer.Serialize(new ServerConfiguration()));
        }
    }
}
