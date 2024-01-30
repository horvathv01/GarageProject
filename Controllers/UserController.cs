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
    public async Task<IActionResult> GetUser( long id )
    {
        try
        {
            var query = await _userService.GetUserById( id );
            var result = _converter.ConvertToUserDTO( query );
            return Ok( result );
        }
        catch ( Exception ex )
        {
            return BadRequest( ex.Message );
        }
    }

    [HttpGet]
    //[Authorize]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var query = await _userService.GetAllUsers();
            var result = _converter.ConvertToUserDTOIEnumerable( query );
            return Ok( result );
        }
        catch ( Exception ex )
        {
            return BadRequest( ex.Message );
        }
    }

    [HttpGet( "email/{email}" )]
    //[Authorize]
    public async Task<IActionResult> GetUserByEmail( string email )
    {
        try
        {
            var query = await _userService.GetUserByEmail( email );
            var result = _converter.ConvertToUserDTO( query );
            return Ok( result );
        }
        catch ( Exception ex )
        {
            return BadRequest( ex.Message );
        }
    }

    [HttpGet( "allmanagers" )]
    //[Authorize]
    public async Task<IActionResult> GetAllManagers()
    {
        try
        {
            var query = await _userService.GetAllManagers();
            var result = _converter.ConvertToUserDTOIEnumerable( query );
            return Ok( result );
        }
        catch ( Exception ex )
        {
            return BadRequest( ex.Message );
        }
    }


    [HttpPut( "{id}" )]
    [Authorize]
    public async Task<IActionResult> UpdateUser( [FromBody] UserDTO newUser, long id )
    {
        try
        {
            var loggedInUserId = GetLoggedInUserId();
            var result = await _userService.UpdateUser( id, newUser, loggedInUserId );
            if ( result )
            {
                return Ok( $"User with id {id} has been updated successfully" );
            }

            return BadRequest( "Something went wrong" );
        }
        catch ( Exception ex )
        {
            return BadRequest( ex.Message );
        }
    }

    [HttpDelete( "{id}" )]
    [Authorize]
    public async Task<IActionResult> DeleteUser( long id )
    {
        try
        {
            var loggedInUserId = GetLoggedInUserId();
            var result = await _userService.DeleteUser( id, loggedInUserId );
            if ( result )
            {
                return Ok( $"User with id {id} has been successfully deleted." );
            }
            return BadRequest( "Something went wrong" );
        }
        catch ( Exception ex )
        {
            return BadRequest( ex.Message );
        }
    }

    private long GetLoggedInUserId()
    {
        long userId;
        long.TryParse( HttpContext?.User?.Claims?.FirstOrDefault( claim => claim.Type == ClaimTypes.Authentication )?.Value, out userId );
        return userId;
    }
}