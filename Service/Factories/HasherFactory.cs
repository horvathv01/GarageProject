using Microsoft.AspNetCore.Identity;

namespace GarageProject.Service.Factories;

public class HasherFactory : IHasherFactory
{
    public PasswordHasher<string> GetHasher()
    {
        return new PasswordHasher<string>();
    }
}