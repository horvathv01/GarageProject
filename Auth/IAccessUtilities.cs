using Microsoft.AspNetCore.Identity;
using GarageProject.Models;

namespace GarageProject.Auth;

public interface IAccessUtilities
{
    string HashPassword(string password, string userEmail);
    PasswordVerificationResult Authenticate(string email, string hashedPassword, string password);
}