using MSFEP.Database.Interfaces;
using Npgsql;
using System.Data;

namespace MSFEP.Database;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string connectionString;

    public DbConnectionFactory(string connectionString)
    {
        this.connectionString = connectionString;
    }

    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(connectionString);
        connection.Open();
        return connection;
    }

    public async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync();
        return connection;
    }
}