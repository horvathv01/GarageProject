using GarageProject.Models;

namespace GarageProject.Service;

public interface IManagerService
{
    Task<bool> AddManager(Manager manager);
    
    Task<Manager?> GetManagerById(long id);
    Task<Manager?> GetManagerByEmail(string email);
    Task<List<Manager>> GetAllManagers();
    
    Task<List<Manager>> GetListOfManagers(List<long> ids);
    
    Task<bool> UpdateManager(long id, Manager manager, long loggedInUserId );
    Task<bool> UpdateManager(long id, UserDTO manager, long loggedInUserId );
}