using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GarageProject.Models;
using GarageProject.Models.Enums;
using GarageProject.Service;
using PsychAppointments_API.Converters;

namespace GarageProject.Controllers;

[ApiController, Route( "user" )]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserConverter _converter;

    public UserController(
    IUserService userService,
    IUserConverter converter
    )
    {
        _userService = userService;
        _converter = converter;
    }

    [HttpGet( "{id}" )]
    //[Authorize]
    public async Task<UserDTO?> GetUser( long id )
    {
        var result = await _userService.GetUserById( id );
        return _converter.ConvertToUserDTO( result );
    }

    [HttpGet]
    //[Authorize]
    public async Task<IEnumerable<UserDTO>?> GetAllUsers()
    {

        var result = await _userService.GetAllUsers();
        return _converter.ConvertToUserDTOIEnumerable( result );
    }

    [HttpGet( "email/{email}" )]
    //[Authorize]
    public async Task<UserDTO?> GetUserByEmail( string email )
    {

        var result = await _userService.GetUserByEmail( email );
        return _converter.ConvertToUserDTO( result );
    }

    [HttpGet( "allmanagers" )]
    //[Authorize]
    public async Task<IEnumerable<UserDTO>?> GetAllManagers()
    {
        var result = await _userService.GetAllManagers();
        return _converter.ConvertToUserDTOIEnumerable( result );
    }


    [HttpPut( "{id}" )]
    [Authorize]
    public async Task<IActionResult> UpdateUser( [FromBody] UserDTO newUser, long id )
    {
        var loggedInUser = await GetLoggedInUser();
        var result = await _userService.UpdateUser( id, newUser, loggedInUser );
        if ( result )
        {
            return Ok( $"User with id {id} has been updated successfully" );
        }

        return BadRequest( "Something went wrong" );
    }

    [HttpDelete( "{id}" )]
    [Authorize]
    public async Task<IActionResult> DeleteUser( long id )
    {
        var user = await GetLoggedInUser();
        var result = await _userService.DeleteUser( id, user );
        if ( result )
        {
            return Ok( $"User with id {id} has been successfully deleted." );
        }
        return BadRequest( "Something went wrong" );
    }

    private async Task<User?> GetLoggedInUser()
    {
        long userId;
        long.TryParse( HttpContext?.User?.Claims?.FirstOrDefault( claim => claim.Type == ClaimTypes.Authentication )?.Value, out userId );
        return await _userService.GetUserById( userId );
    }
}