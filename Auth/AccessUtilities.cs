using Microsoft.AspNetCore.Identity;
using GarageProject.Service.Factories;

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
}