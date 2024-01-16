using GarageProject.Models;

namespace GarageProject.Service;

public interface IUserService
{
     Task<bool> AddUser(UserDTO user);
     Task<User?> GetUserById(long id);
     Task<User?> GetUserByEmail(string email);
     Task<IEnumerable<User>> GetAllUsers();
     Task<IEnumerable<Manager>> GetAllManagers();
     Task<IEnumerable<User>> GetListOfUsers(List<long> ids);
     Task<bool> UpdateUser(long id, UserDTO newUser);
     Task<bool> DeleteUser(long id);
}