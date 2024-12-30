using Dapper;
using MSFEP.DataAccess.Interfaces;
using MSFEP.Database.Interfaces;
using MSFEP.Models;

namespace MSFEP.DataAccess;

public class TimeTrackDataAccess : ITimeTrackDataAccess
{
    private readonly IDbConnectionFactory dbConnectionFactory;

    public TimeTrackDataAccess(IDbConnectionFactory dbConnectionFactory)
    {
        this.dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<TimeTrackEntry>> GetEntriesForMonthAsync(string userPrincipalName, int month, int year)
    {
        try
        {
            using var dbConnection = await dbConnectionFactory.CreateConnectionAsync();
            var sql = @"
            SELECT id, userprincipalname, workday, hours, projectId 
            FROM timetrackentry 
            WHERE userprincipalname = @UserPrincipalName 
            AND EXTRACT(MONTH FROM workday) = @Month 
            AND EXTRACT(YEAR FROM workday) = @Year
            ORDER BY workday";

            var parameters = new
            {
                UserPrincipalName = userPrincipalName,
                Month = month,
                Year = year
            };

            var timeTrackEntries = await dbConnection.QueryAsync<TimeTrackEntry>(sql, parameters);
            return timeTrackEntries.AsList();
        }
        catch (Exception ex)
        {
            throw new DataAccessException("An error occurred when trying to get timetrackentries for the specified month.", ex);
        }
    }


    public async Task<IEnumerable<TimeTrackEntry>> GetAsync(int startIndex, int count, string sortColumn, bool ascending, Dictionary<string, string> filters)
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

            var sql = $"SELECT id, userPrincipalName, workday, hours, projectId FROM timetrackentry {whereClause} {orderByClause} OFFSET {startIndex} LIMIT {count}";

            var timetrackentires = await dbConnection.QueryAsync<TimeTrackEntry>(sql);
            return timetrackentires.AsList();
        }
        catch (Exception ex)
        {
            throw new DataAccessException("An error occurred when trying to get timetrackentries.", ex);
        }
    }

    public async Task<int> GetCountAsync()
    {
        try
        {
            using var dbConnection = await dbConnectionFactory.CreateConnectionAsync();
            var sql = "SELECT COUNT(*) FROM timetrackentry";
            var result = await dbConnection.ExecuteScalarAsync<int>(sql);
            return result;
        }
        catch (Exception ex)
        {
            throw new DataAccessException("An error occurred when trying to count all timetrackentries.", ex);
        }
    }

    public async Task<int> CreateAsync(TimeTrackEntryCreate givenTimeTrackEntryCreate)
    {
        try
        {
            using var dbConnection = await dbConnectionFactory.CreateConnectionAsync();
            var sql = "INSERT INTO timetrackentry (userPrincipalName, workday, hours, projectId) VALUES(@UserPrincipalName, @Workday, @Hours, @ProjectId);";
            var parameters = new
            {
                UserPrincipalName = givenTimeTrackEntryCreate.UserPrincipalName,
                Workday = givenTimeTrackEntryCreate.Workday,
                Hours = givenTimeTrackEntryCreate.Hours,
                ProjectId = givenTimeTrackEntryCreate.ProjectId
            };

            var affectedRows = await dbConnection.ExecuteAsync(sql, parameters);
            return affectedRows;
        }
        catch (Exception ex)
        {
            throw new DataAccessException("An error occurred when trying to create timetrackentry.", ex);
        }
    }
}
