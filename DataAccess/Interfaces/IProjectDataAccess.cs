using MSFEP.Models;

namespace MSFEP.DataAccess.Interfaces;

public interface IProjectDataAccess
{
    Task<int> CreateAsync(ProjectCreate givenProjectCreate);
    Task<IEnumerable<Project>> GetAll();
    Task<IEnumerable<Project>> GetAsync(int startIndex, int count, string sortColumn, bool ascending, Dictionary<string, string> filters);
    Task<int> GetCountAsync();
}