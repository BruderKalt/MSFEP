using Dapper;
using MSFEP.DataAccess.Interfaces;
using MSFEP.Database.Interfaces;
using MSFEP.Models;

namespace MSFEP.DataAccess
{
    public class ProjectAffiliationRequestDataAccess : IProjectAffiliationRequestDataAccess
    {
        private readonly IDbConnectionFactory dbConnectionFactory;

        public ProjectAffiliationRequestDataAccess(IDbConnectionFactory dbConnectionFactory)
        {
            this.dbConnectionFactory = dbConnectionFactory;
        }

        public async Task<IEnumerable<ProjectAffiliationRequest>> GetByProjectNameAsync(string projectName)
        {
            try
            {
                using var dbConnection = await dbConnectionFactory.CreateConnectionAsync();

                var sql = $"SELECT id, projectname, name, role, githubusername, userprincipalname, isgranted FROM projectaffiliationrequest WHERE projectname = @ProjectName";

                var parameters = new
                {
                    ProjectName = projectName,
                };

                var projects = await dbConnection.QueryAsync<ProjectAffiliationRequest>(sql);
                return projects.AsList();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("An error occurred when trying to get projectAffiliationRequests for a certain projectName.", ex);
            }
        }

        public async Task<int> CreateAsync(ProjectAffiliationRequestCreate givenProjectAffiliationRequestCreate)
        {
            try
            {
                using var dbConnection = await dbConnectionFactory.CreateConnectionAsync();
                var sql = "INSERT INTO projectaffiliationrequest (projectname, name, role, githubusername, userprincipalname, isgranted) VALUES(@ProjectName, @Name, @Role, @GitHubUsername, @UserPrincipalName, @IsGranted);";
                var parameters = new
                {
                    ProjectName = givenProjectAffiliationRequestCreate.ProjectName,
                    Name = givenProjectAffiliationRequestCreate.Name,
                    Role = givenProjectAffiliationRequestCreate.Role,
                    GitHubUsername = givenProjectAffiliationRequestCreate.GitHubUsername,
                    UserPrincipalName = givenProjectAffiliationRequestCreate.UserPrincipalName,
                    IsGranted = false,
                };

                var affectedRows = await dbConnection.ExecuteAsync(sql, parameters);
                return affectedRows;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("An error occurred when trying to create projectAffiliationRequest.", ex);
            }
        }

        public async Task<int> GrantIssuanceAsync(int id)
        {
            try
            {
                using var dbConnection = await dbConnectionFactory.CreateConnectionAsync();

                var sql = "UPDATE projectaffiliationrequest SET isgranted = true WHERE id = @Id;";

                var parameters = new
                {
                    Id = id
                };

                var affectedRows = await dbConnection.ExecuteAsync(sql, parameters);
                return affectedRows;
            }
            catch (Exception ex)
            {
                throw new DataAccessException("An error occurred when trying to grant a projectAffiliationRequest.", ex);
            }
        }

        public async Task<bool> CheckGrantAsync(string userPrincipalName, string projectName, string name, string gitHubUsername, string role)
        {
            try
            {
                using var dbConnection = await dbConnectionFactory.CreateConnectionAsync();

                var sql = @"SELECT isgranted FROM projectaffiliationrequest 
                                WHERE userprincipalname = @UserPrincipalName
                                AND projectname = @ProjectName
                                AND name = @Name
                                AND githubusername = @GitHubUsername
                                AND role = @Role";

                var parameters = new
                {
                    UserPrincipalName = userPrincipalName,
                    ProjectName = projectName,
                    Name = name,
                    GitHubUsername = gitHubUsername,
                    Role = role,
                };

                var isgranted = await dbConnection.QueryAsync<bool>(sql, parameters);
                return isgranted.FirstOrDefault();
            }
            catch (Exception ex)
            {
                throw new DataAccessException("An error occurred when trying to check projectAffiliationRequest.", ex);
            }
        }
    }
}
