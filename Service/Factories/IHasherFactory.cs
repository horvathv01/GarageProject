using Microsoft.AspNetCore.Identity;

namespace GarageProject.Service.Factories
{
    public interface IHasherFactory
    {
        PasswordHasher<string> GetHasher();
    }
}