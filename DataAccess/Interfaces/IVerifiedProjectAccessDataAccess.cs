using MSFEP.Models;

namespace MSFEP.DataAccess.Interfaces
{
    public interface IVerifiedProjectAccessDataAccess
    {
        Task<int> CreateAsync(VerifiedProjectAccessCreate givenVerifiedProjectAccessCreate);
        Task<int> DeleteAll(int id);
        Task<IEnumerable<VerifiedProjectAccess>> GetAllAsync();
    }
}