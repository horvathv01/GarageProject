using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GarageProject.Models;
using GarageProject.Models.Enums;
using GarageProject.Service;

namespace GarageProject.Controllers;

[ApiController, Route( "user" )]
public class UserController : ControllerBase
{
    private IUserService _userService;

    public UserController(
    IUserService userService
    )
    {
        _userService = userService;
    }

    [HttpGet( "{id}" )]
    [Authorize]
    public async Task<UserDTO?> GetUser( long id )
    {
        var result = await _userService.GetUserById( id );
        if ( result != null )
        {
            return new UserDTO( result );
        }
        return null;
    }

    [HttpGet]
    [Authorize]
    public async Task<List<UserDTO>?> GetAllUsers()
    {

        var result = await _userService.GetAllUsers();
        if ( result != null )
        {
            return result.Select( u => new UserDTO( u ) )
                .ToList();
        }
        return null;
    }

    [HttpGet( "email/{email}" )]
    [Authorize]
    public async Task<UserDTO?> GetUserByEmail( string email )
    {

        var result = await _userService.GetUserByEmail( email );
        if ( result != null )
        {
            return new UserDTO( result );
        }
        return null;
    }

    [HttpGet( "allmanagers" )]
    [Authorize]
    public async Task<List<UserDTO>?> GetAllManagers()
    {
        var result = await _userService.GetAllManagers();
        if ( result != null )
        {
            return result.Select( u => new UserDTO( u ) )
                .ToList();
        }
        return null;
    }


    [HttpPut( "{id}" )]
    [Authorize]
    public async Task<IActionResult> UpdateUser( [FromBody] UserDTO updatedUser, long id )
    {
        var result = await _userService.UpdateUser( id, updatedUser );
        if ( result )
        {
            return Ok( $"User with id {id} has been successfully updated." );
        }
        return BadRequest( "Something went wrong" );
    }

    [HttpDelete( "{id}" )]
    [Authorize]
    public async Task<IActionResult> DeleteUser( long id )
    {
        var user = await GetLoggedInUser();
        if ( user != null )
        {
            if ( user.Id == id || user.Type == UserType.Manager )
            {
                return Unauthorized( "You are not authorized to delete this user." );
            }

            var result = await _userService.DeleteUser( id );
            if ( result )
            {
                return Ok( $"User with id {id} has been successfully deleted." );
            }
            return BadRequest( "Something went wrong" );
        }
        return Unauthorized( "Logged in user could not be retreived." );
    }

    private async Task<User?> GetLoggedInUser()
    {
        long userId;
        long.TryParse( HttpContext?.User?.Claims?.FirstOrDefault( claim => claim.Type == ClaimTypes.Authentication )?.Value, out userId );
        return await _userService.GetUserById( userId );
    }

}