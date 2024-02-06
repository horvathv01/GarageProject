using Microsoft.AspNetCore.Identity;
using GarageProject.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace GarageProject.Auth;

public interface IAccessUtilities
{
    string HashPassword(string password, string userEmail);
    PasswordVerificationResult Authenticate(string email, string hashedPassword, string password);
    /// <summary>
    /// Decodes base64 encoded string, separates string into two parts at separator symbol: ":", 
    /// then returns parts as Tuple<string, string>. First part is supposed to be the email address, and the second one should be the password.
    /// </summary>
    /// <param name="authorizationHeader"></param>
    /// <paramref name="separator"/>
    /// The separator character in string format, which 
    /// <returns></returns>
    Tuple<string, string> GetUserNameAndPassword( string authorizationHeader, string? separator = ":" );
    ClaimsPrincipal GenerateClaimsPrincipal( User user );
    AuthenticationProperties GenerateAuthenticationProperties( int num = 1 );
}