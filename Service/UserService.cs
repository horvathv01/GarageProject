using System.Data;
using Microsoft.EntityFrameworkCore;
using GarageProject.Auth;
using GarageProject.DAL;
using GarageProject.Models;
using GarageProject.Models.Enums;
using System.Net;
using GarageProject.Converters;

namespace GarageProject.Service;

public class UserService : IUserService
{
    private readonly GarageProjectContext _context;
    private readonly IAccessUtilities _hasher;
    private readonly IServiceProvider _serviceProvider;
    public UserService(
        GarageProjectContext context,
        IAccessUtilities hasher,
        IServiceProvider serviceProvider )
    {
        _context = context;
        _hasher = hasher;
        _serviceProvider = serviceProvider;
    }

    public async Task<bool> AddUser( UserDTO user )
    {
        string password = _hasher.HashPassword( user.Password, user.Email );

        var dateTimeConverter = _serviceProvider.GetService<IDateTimeConverter>();
        if ( dateTimeConverter == null )
        {
            throw new Exception( "Dependency injection failed." );
        }
        var birthDate = dateTimeConverter.Convert( user.DateOfBirth );

        if ( user.Type == Enum.GetName( typeof( UserType ), UserType.Manager ) )
        {
            var newManager = new Manager( user.Name, user.Email, user.Phone, birthDate, password );
            await _context.Managers.AddAsync( newManager );
            await _context.SaveChangesAsync();
            return true;
        }
        else
        {
            var newUser = new User( user.Name, user.Email, user.Phone, birthDate, password );
            await _context.Users.AddAsync( newUser );
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public async Task<User?> GetUserById( long id )
    {
        var user = await _context.Users
            .Include( u => u.Bookings )
            .FirstOrDefaultAsync( cli => cli.Id == id );
        if ( user != null && user.Type == UserType.Manager )
        {
            return user as Manager;
        }
        return user;
    }

    public async Task<User?> GetUserByEmail( string email )
    {
        var user = await _context.Users
            .Include( u => u.Bookings )
            .FirstOrDefaultAsync( cli => cli.Email == email );
        if ( user != null && user.Type == UserType.Manager )
        {
            return user as Manager;
        }
        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsers()
    {
        return await _context.Users
            .Include(u => u.Bookings)
            .ToListAsync();
    }

    public async Task<IEnumerable<Manager>> GetAllManagers()
    {
        return await _context.Managers
            .Include( u => u.Bookings )
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetListOfUsers( List<long> ids )
    {
        var users = await _context.Users.Where( cli => ids.Contains( cli.Id ) )
            .Include( u => u.Bookings )
            .ToListAsync();
        var managers = await _context.Managers.Where( man => ids.Contains( man.Id ) )
            .Include( u => u.Bookings )
            .ToListAsync();

        return users.Cast<User>()
            .Union( managers );
    }

    public async Task<bool> UpdateUser( long id, UserDTO newUser, User? loggedInUser = null )
    {
        if ( loggedInUser == null )
        {
            throw new InvalidOperationException( "Logged in user could not be retrieved" );
        }

        if ( !IsUserAuthorizedToHandleUser( loggedInUser, id ) )
        {
            throw new UnauthorizedAccessException( "You are not authorized to update this user." );
        }

        var user = await GetUserById( id );
        if ( user == null )
        {
            throw new InvalidOperationException( "User not found in database." );
        }

        string password = _hasher.HashPassword( newUser.Password, newUser.Email );

        var dateTimeConverter = _serviceProvider.GetService<IDateTimeConverter>();
        if ( dateTimeConverter == null )
        {
            throw new Exception( "Dependency injection failed." );
        }
        var newBirthDay = dateTimeConverter.Convert( newUser.DateOfBirth );

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

    public async Task<bool> DeleteUser( long id, User? loggedInUser = null )
    {
        if ( !IsUserAuthorizedToHandleUser( loggedInUser, id ) )
        {
            throw new UnauthorizedAccessException( "You are not authorized to update this user." );
        }
        var user = await GetUserById( id );
        if ( user == null )
        {
            throw new InvalidOperationException( $"User with id {id} was not found." );
        }
        _context.Remove( user );
        await _context.SaveChangesAsync();
        return true;
    }

    private bool IsUserAuthorizedToHandleUser( User? loggedInUser, long otherUserId )
    {
        return loggedInUser != null && ( loggedInUser.Id == otherUserId || loggedInUser.Type == UserType.Manager );
    }
}
