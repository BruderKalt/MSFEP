using System.Data;

namespace MSFEP.Database.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
    Task<IDbConnection> CreateConnectionAsync();
}