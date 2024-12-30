using Dapper;
using MSFEP.DataAccess.Interfaces;
using MSFEP.Database.Interfaces;
using MSFEP.Models;

namespace MSFEP.DataAccess;

public class ProjectDataAccess : IProjectDataAccess
{
    private readonly IDbConnectionFactory dbConnectionFactory;

    public ProjectDataAccess(IDbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<Project>> GetAsync(int startIndex, int count, string sortColumn, bool ascending, Dictionary<string, string> filters)
    {
        try
        {
            using var dbConnection = await dbConnectionFactory.CreateConnectionAsync();
            var orderByClause = $"ORDER BY {sortColumn} {(ascending ? "ASC" : "DESC")}";

            var whereClauses = new List<string>();
            foreach (var filter in filters.Skip(4))
            {
                whereClauses.Add($"a.{filter.Key} LIKE '%{filter.Value}%'");
            }

            var whereClause = whereClauses.Any() ? "WHERE " + string.Join(" AND ", whereClauses) : string.Empty;

            var sql = $"SELECT id, name, manager FROM project {whereClause} {orderByClause} OFFSET {startIndex} LIMIT {count}";

            var projects = await dbConnection.QueryAsync<Project>(sql);
            return projects.AsList();
        }
        catch (Exception ex)
        {
            throw new DataAccessException("An error occurred when trying to get projects.", ex);
        }
    }

    public async Task<int> GetCountAsync()
    {
        try
        {
            using var dbConnection = await dbConnectionFactory.CreateConnectionAsync();
            var sql = "SELECT COUNT(*) FROM project";
            var result = await dbConnection.ExecuteScalarAsync<int>(sql);
            return result;
        }
        catch (Exception ex)
        {
            throw new DataAccessException("An error occurred when trying to count all projects.", ex);
        }
    }

    public async Task<IEnumerable<Project>> GetAll()
    {
        try
        {
            using var dbConnection = await dbConnectionFactory.CreateConnectionAsync();

            var sql = $"SELECT id, name, manager FROM project";

            var projects = await dbConnection.QueryAsync<Project>(sql);
            return projects.AsList();
        }
        catch (Exception ex)
        {
            throw new DataAccessException("An error occurred when trying to get all projects.", ex);
        }
    }

    public async Task<int> CreateAsync(ProjectCreate givenProjectCreate)
    {
        try
        {
            using var dbConnection = await dbConnectionFactory.CreateConnectionAsync();
            var sql = "INSERT INTO project (name, manager) VALUES(@Name, @Manager);";
            var parameters = new
            {
                Name = givenProjectCreate.Name,
                Manager = givenProjectCreate.Manager,
            };

            var affectedRows = await dbConnection.ExecuteAsync(sql, parameters);
            return affectedRows;
        }
        catch (Exception ex)
        {
            throw new DataAccessException("An error occurred when trying to create project.", ex);
        }
    }
}
