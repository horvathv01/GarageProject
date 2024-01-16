using System.Data;
using Microsoft.EntityFrameworkCore;
using GarageProject.Auth;
using GarageProject.DAL;
using GarageProject.Models;
using GarageProject.Models.Enums;
using System.Net;

namespace GarageProject.Service;

public class UserService : IUserService
{
    private readonly GarageProjectContext _context;
    private readonly IAccessUtilities _hasher;
    
    public UserService(
        GarageProjectContext context,
        IAccessUtilities hasher
        )
    {
        _context = context;
        _hasher = hasher;
    }
    
    
    public async Task<bool> AddUser(UserDTO user)
    {
        string password = _hasher.HashPassword(user.Password, user.Email);

        DateTime birthDate = DateTime.MinValue;
        DateTime.TryParse(user.DateOfBirth, out birthDate);
        birthDate = DateTime.SpecifyKind(birthDate, DateTimeKind.Utc);
        
        try
        {
            if (user.Type == Enum.GetName(typeof(UserType), UserType.Manager))
            {
                var newManager = new Manager(user.Name, user.Email, user.Phone, birthDate, password);
                await _context.Managers.AddAsync(newManager);
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                var newUser = new User(user.Name, user.Email, user.Phone, birthDate, password);
                await _context.Users.AddAsync( newUser );
                await _context.SaveChangesAsync();
                return true;
            }

            
        }
        catch (Exception e)
        {
            Console.WriteLine("Adding new user failed.");
            Console.WriteLine(e);
            return false;
        }
        
        
    }

    public async Task<User?> GetUserById(long id)
    {
        var user = await _context.Users.FirstOrDefaultAsync(cli => cli.Id == id);
        if ( user != null && user.Type == UserType.Manager )
        {
            return user as Manager;
        }
        return user;
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync( cli => cli.Email == email);
        if ( user != null && user.Type == UserType.Manager )
        {
            return user as Manager;
        }
        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _context.Users
            .ToListAsync();
    }

    public async Task<IEnumerable<Manager>> GetAllManagers()
    {
        return await _context.Managers
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetListOfUsers(List<long> ids)
    {
        var users = await _context.Users.Where(cli => ids.Contains(cli.Id)).ToListAsync();
        var managers = await _context.Managers.Where(man => ids.Contains(man.Id)).ToListAsync();

        return users.Cast<User>()
            .Union( managers );
    }

    public async Task<bool> UpdateUser( long id, UserDTO newUser )
    {
        var user = await GetUserById( id );
        if ( user == null )
        {
            return false;
        }
        string password = _hasher.HashPassword( newUser.Password, newUser.Email );

        DateTime newBirthDay = user.DateOfBirth;
        DateTime.TryParse( newUser.DateOfBirth, out newBirthDay );
        newBirthDay = DateTime.SpecifyKind( newBirthDay, DateTimeKind.Utc );

        if ( Enum.GetName( typeof( UserType ), user.Type ) != newUser.Type )
        {
            await AddUser( newUser );
            _context.Remove( user );
            await _context.SaveChangesAsync();
            return true;
        }
        user.Name = newUser.Name;
        user.Email = newUser.Email;
        user.Phone = newUser.Phone;
        user.DateOfBirth = newBirthDay;
        user.Password = password;

        switch ( user.Type )
        {
            case UserType.User:
                _context.Update( user );
                await _context.SaveChangesAsync();
                return true;
            case UserType.Manager:
                _context.Update( user );
                await _context.SaveChangesAsync();
                return true;
            default: return false;
        }
    }

    public async Task<bool> DeleteUser( long id )
    {
        try
        {
            var user = await GetUserById( id );
            if ( user != null )
            {
                _context.Remove( user );
                await _context.SaveChangesAsync();
                return true;
            }

            Console.WriteLine( $"User with id {id} was not found." );
            return false;
        }
        catch ( Exception e )
        {
            Console.WriteLine( $"Removing user with id {id} failed." );
            Console.WriteLine( e );
            return false;
        }
    }
}
   