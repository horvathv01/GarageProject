using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GarageProject.Auth;
using GarageProject.Models;
using GarageProject.Models.Enums;
using GarageProject.Service;
using GarageProject.Converters;

namespace GarageProject.Controllers;

[ApiController, Route("access")]
public class AccessController : ControllerBase
{
    private readonly IAccessUtilities _accessUtilities;
    private readonly IUserService _userService;
    private readonly IUserConverter _converter;
    private readonly ILoggerService _loggerService;

    public AccessController( IAccessUtilities accessUtilities, IUserService userService, IUserConverter converter, ILoggerService loggerService )
    {
        _accessUtilities = accessUtilities;
        _userService = userService;
        _converter = converter;
        _loggerService = loggerService;
    }

    [HttpPost("registration")]
    public async Task<IActionResult> RegisterUser([FromBody] UserDTO user)
    {
        try
        {
            await _userService.AddUser(user);
            return Ok($"{user.Type} {user.Name} has been successfully registered.");
        }
        catch (Exception ex)
        {
            return BadRequest( ex.Message );
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser()
    {
        string? authorizationHeader = HttpContext.Request.Headers["Authorization"];
        if(authorizationHeader == null) 
        {
            return Unauthorized("Authorization header is missing.");
        }

        var credentials = _accessUtilities.GetUserNameAndPassword(authorizationHeader); //item1: email, item2: password
        var user = await _userService.GetUserByEmail(credentials.Item1);
        if (user == null)
        {
            _loggerService.Log( "Authorization failed: user not found in database.");
            return Unauthorized();
        }

        var authenticated = _accessUtilities.Authenticate( credentials.Item1, user.Password, credentials.Item2 );
        if (authenticated == PasswordVerificationResult.Success)
        {
            var claimsPrincipal = _accessUtilities.GenerateClaimsPrincipal( user );
            var authProperties = _accessUtilities.GenerateAuthenticationProperties();

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                authProperties);

            _loggerService.Log( $"Authorization successful,{user.Type} {user.Name} logged in successfully.");
            var loggedInUser = _converter.ConvertToUserDTO( user );
            if(loggedInUser != null) //this is only here to suppress warning in IDE as loggedInUser cannot be null at this point
            {
                loggedInUser.Password = "";
            }
            return Ok( loggedInUser );
        }
        _loggerService.Log( $"Authorization failed for {user.Type} {user.Name}: password mismatch");
        return Unauthorized();
    }
    
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutUser()
    {
        try
        {
            var userName = HttpContext.User.Identity?.Name;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            string message = $"{userName} logged out successfully.";
            _loggerService.Log( message);
            return Ok(message);
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Logout failed: {e.Message}");
        }
    }
}