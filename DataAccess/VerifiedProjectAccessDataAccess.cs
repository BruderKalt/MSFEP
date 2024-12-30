using Dapper;
using MSFEP.DataAccess.Interfaces;
using MSFEP.Database.Interfaces;
using MSFEP.Models;

namespace MSFEP.DataAccess
{
    public class VerifiedProjectAccessDataAccess : IVerifiedProjectAccessDataAccess
    {
        private readonly IDbConnectionFactory dbConnectionFactory;

        public VerifiedProjectAccessDataAccess(IDbConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<VerifiedProjectAccess>> GetAllAsync()
        {
            try
            {
                using var dbConnection = await dbConnectionFactory.CreateConnectionAsync();

                var sql = $"SELECT * FROM verifiedprojectaccess";

                var projects = await dbConnection.QueryAsync<VerifiedProjectAccess>(sql);
                return projects.AsList();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("An error occurred when trying to get all verifiedProjectAccess.", ex);
            }
        }

        public async Task<int> CreateAsync(VerifiedProjectAccessCreate givenVerifiedProjectAccessCreate)
        {
            try
            {
                using var dbConnection = await dbConnectionFactory.CreateConnectionAsync();
                var sql = "INSERT INTO verifiedprojectaccess (id, userprincipalname, projectname, githubusername, role) VALUES(@UserPrincipalName, @ProjectName, @GitHubUsernam, @Role);";
                var parameters = new
                {
                    UserPrincipalName = givenVerifiedProjectAccessCreate.UserPrincipalName,
                    ProjectName = givenVerifiedProjectAccessCreate.ProjectName,
                    GitHubUsername = givenVerifiedProjectAccessCreate.GitHubUsername,
                    Role = givenVerifiedProjectAccessCreate.Role,
                };

                var affectedRows = await dbConnection.ExecuteAsync(sql, parameters);
                return affectedRows;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("An error occurred when trying to create projectAffiliationRequest.", ex);
            }
        }

        public async Task<int> DeleteAll(int id)
        {
            try
            {
                using var dbConnection = await dbConnectionFactory.CreateConnectionAsync();

                var sql = $"DELETE FROM verifiedprojectaccess";

                var affectedRows = await dbConnection.ExecuteAsync(sql);
                return affectedRows;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("An error occurred when trying to delete all verifiedProjectAccess.", ex);
            }
        }
    }
}
