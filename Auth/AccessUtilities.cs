using Microsoft.AspNetCore.Identity;
using GarageProject.Service.Factories;
using GarageProject.Models.Enums;
using GarageProject.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text;
using GarageProject.Service;

namespace GarageProject.Auth;

public class AccessUtilities : IAccessUtilities
{
    private readonly IHasherFactory _hasherFactory;

    public AccessUtilities(IHasherFactory hasherFactory)
    {
        _hasherFactory = hasherFactory;
    }
    
    public string HashPassword(string password, string userEmail)
    {
        string salt = GetSalt(userEmail);
        var hasher = _hasherFactory.GetHasher();
        return hasher.HashPassword(salt, password);

    }

    public PasswordVerificationResult Authenticate(string email, string hashedPassword, string password)
    {
        string salt = GetSalt(email);
        var hasher = _hasherFactory.GetHasher();
        var result = hasher.VerifyHashedPassword(salt, hashedPassword, password);
        return result;
    }

    public string GetSalt(string userEmail)
    {
        string salt = "";
        var arr = String.Concat(userEmail.OrderBy(ch => ch)).ToArray();
        for (int i = 0; i < 5; i++)
        {
            salt += arr[i];
        }
        return salt;
    }

    public Tuple<string, string> GetUserNameAndPassword( string authorizationHeader )
    {
        var base64String = Convert.FromBase64String( authorizationHeader );
        var credentials = Encoding.UTF8.GetString( base64String );
        var parts = credentials.Split( ":" );
        var email = parts[0];
        var pass = parts[1];

        return new Tuple<string, string>( email, pass );
    }

    public ClaimsPrincipal GenerateClaimsPrincipal( User user )
    {
        var claims = GenerateClaims( user );
        var claimsIdentity = GenerateClaimsIdentity( claims );
        return new ClaimsPrincipal( claimsIdentity );
    }

    private List<Claim> GenerateClaims( User user )
    {
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Authentication, user.Id.ToString())
            };
        var roleName = Enum.GetName( typeof( UserType ), user.Type );
        claims.Add( new Claim( ClaimTypes.Role, roleName ?? throw new InvalidOperationException( "Invalid role name" ) ) );

        return claims;
    }

    private ClaimsIdentity GenerateClaimsIdentity( List<Claim> claims )
    {
        return new ClaimsIdentity( claims, CookieAuthenticationDefaults.AuthenticationScheme );
    }

    public AuthenticationProperties GenerateAuthenticationProperties( int num = 1 )
    {
        return new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddDays( num )
        };
    }
}