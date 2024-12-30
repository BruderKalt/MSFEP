using MSFEP.Models;

namespace MSFEP.DataAccess.Interfaces;

public interface ITimeTrackDataAccess
{
    Task<IEnumerable<TimeTrackEntry>> GetEntriesForMonthAsync(string userPrincipalName, int month, int year);
    Task<int> CreateAsync(TimeTrackEntryCreate givenTimeTrackEntryCreate);
    Task<IEnumerable<TimeTrackEntry>> GetAsync(int startIndex, int count, string sortColumn, bool ascending, Dictionary<string, string> filters);
    Task<int> GetCountAsync();
}