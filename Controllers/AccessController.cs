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

namespace GarageProject.Controllers;

[ApiController, Route("access")]
public class AccessController : ControllerBase
{
    private readonly IAccessUtilities _hasher;
    private readonly IUserService _userService;

    public AccessController(IAccessUtilities hasher, IUserService userService)
    {
        _hasher = hasher;
        _userService = userService;
    }

    [HttpPost("registration")]
    public async Task<IActionResult> RegisterUser([FromBody] UserDTO user)
    {
        //check if user has already registered with this email
        var registeredUser = await _userService.GetUserByEmail(user.Email);
        Console.WriteLine(user);
        if (registeredUser != null)
        {
            Console.WriteLine(registeredUser);
            return Conflict("This email has already been registered.");
        }

        //user.Type = Enum.GetName(typeof(UserType), UserType.Client);
        await _userService.AddUser(user);
        
        return Ok($"{user.Type} {user.Name} has been successfully registered.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser()
    {
        string authorizationHeader = HttpContext.Request.Headers["Authorization"];

        var base64String = Convert.FromBase64String(authorizationHeader);
        var credentials = Encoding.UTF8.GetString(base64String);
        var parts = credentials.Split(":");
        var email = parts[0];
        var pass = parts[1];
        var user = await _userService.GetUserByEmail(email);
        
        if (user == null)
        {
            Console.WriteLine("Authorization failed: user == null");
            return Unauthorized();
        }

        var authenticated = _hasher.Authenticate(email, user.Password, pass);

        if (authenticated == PasswordVerificationResult.Success)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Authentication, user.Id.ToString())
            };
            var roleName = Enum.GetName(typeof(UserType), user.Type);
            claims.Add(new Claim(ClaimTypes.Role, roleName ?? throw new InvalidOperationException("Invalid role name")));

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal,
                authProperties);
            
            Console.WriteLine($"Authorization successful,{user.Type} {user.Name} logged in successfully.");
            UserDTO loggedInUser = new UserDTO(user);
            loggedInUser.Password = "";
            
            return Ok(loggedInUser);    
        }
        Console.WriteLine("Authorization failed: password mismatch");
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
            Console.WriteLine($"{userName} logged out successfully.");
            return Ok($"{userName} logged out successfully");
        }
        catch (Exception e)
        {
            return StatusCode(500, $"Logout failed: {e.Message}");
        }
    }

    [HttpPut("update/{id}")]
    [Authorize]
    public async Task<IActionResult> UpdateUser([FromBody] UserDTO newUser, long id)
    {
        var loggedInUser = await GetLoggedInUser();

        if( loggedInUser == null )
        {
            return BadRequest( "Logged in user could not be retrieved" );
        }

        //if a non-manager user attempts to update a user that is not him/herself
        if( !IsUserAuthorizedToHandleUser(loggedInUser, id) )
        {
            return BadRequest( "You are not authorized to update this user." );
        }
        
        var result = await _userService.UpdateUser(id, newUser);
        if (result)
        {
            return Ok($"User with id {id} has been updated successfully");    
        }

        return BadRequest("Something went wrong");
    }

    private async Task<User?> GetLoggedInUser()
    {
        long userId;
        long.TryParse( HttpContext?.User?.Claims?.FirstOrDefault( claim => claim.Type == ClaimTypes.Authentication )?.Value, out userId );
        return await _userService.GetUserById( userId );
    }

    private bool IsUserAuthorizedToHandleUser( User? loggedInUser, long otherUserId)
    {
        return loggedInUser != null && ( loggedInUser.Id == otherUserId || loggedInUser.Type == UserType.Manager );
    }


}