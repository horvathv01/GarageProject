using Microsoft.EntityFrameworkCore;
using GarageProject.Auth;
using GarageProject.DAL;
using GarageProject.Models;
using GarageProject.Models.Enums;

namespace GarageProject.Service;

public class ManagerService : IManagerService
{
    private readonly GarageProjectContext _context;
    private readonly IUserService _userService;
    private readonly IAccessUtilities _hasher;
    
    public ManagerService(
        GarageProjectContext context,
        IUserService userService,
        IAccessUtilities hasher
    )
    {
        _context = context;
        _userService = userService;
        _hasher = hasher;
    }
    
    public async Task<bool> AddManager(Manager manager)
    {
        var alreadyExisting = await _userService.GetUserById(manager.Id);
        if (alreadyExisting != null && manager.Email == alreadyExisting.Email)
        {
            Console.WriteLine($"Manager {manager.Name} is already present in the DB.");
            return false;
        }
        try
        {
            await _userService.AddUser(new UserDTO(manager));
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    public async Task<Manager?> GetManagerById(long id)
    {
        try
        {
            var user = await _userService.GetUserById(id);
            if (user != null)
            {
                return (Manager)user;    
            }
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<Manager?> GetManagerByEmail(string email)
    {
        try
        {
            var user = await _userService.GetUserByEmail(email);
            if (user != null)
            {
                return (Manager)user;    
            }
            return null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return null;
        }
    }

    public async Task<List<Manager>> GetAllManagers()
    {
        return await _context.Managers
            .ToListAsync();
    }

    public async Task<List<Manager>> GetListOfManagers(List<long> ids)
    {
        return await _context.Managers
            .Where(man => ids.Contains(man.Id))
            .ToListAsync();
    }

    public async Task<bool> UpdateManager(long id, Manager manager, long loggedInUserId )
    {
        return await _userService.UpdateUser(id, new UserDTO(manager), loggedInUserId );
    }

    public async Task<bool> UpdateManager(long id, UserDTO manager, long loggedInUserId )
    {
        if (manager.Type != Enum.GetName(typeof(UserType), UserType.Manager))
        {
            return false;
        }

        try
        {
            var original = await _userService.GetUserById(manager.Id);
            if (original == null)
            {
                Console.WriteLine($"Manager {manager.Name} not found in DB.");
                return false;
            }

            DateTime birthDay = DateTime.MinValue;
            DateTime.TryParse(manager.DateOfBirth, out birthDay);
            string password = _hasher.HashPassword(manager.Password, manager.Email);
        
            original.Name = manager.Name;
            original.Email = manager.Email;
            original.Phone = manager.Phone;
            original.DateOfBirth = DateTime.SpecifyKind(birthDay, DateTimeKind.Utc);
            original.Password = password;
            _context.Update(original);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}