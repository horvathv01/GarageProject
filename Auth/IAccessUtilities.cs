using Microsoft.AspNetCore.Identity;
using GarageProject.Models;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace GarageProject.Auth;

public interface IAccessUtilities
{
    string HashPassword(string password, string userEmail);
    PasswordVerificationResult Authenticate(string email, string hashedPassword, string password);
    Tuple<string, string> GetUserNameAndPassword( string authorizationHeader );
    ClaimsPrincipal GenerateClaimsPrincipal( User user );
    AuthenticationProperties GenerateAuthenticationProperties( int num = 1 );
}