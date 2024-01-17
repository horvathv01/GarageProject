using GarageProject.Models.Enums;
using GarageProject.Models;

namespace GarageProject.Models;

public class Manager : User
{
    
    public Manager(string name, 
        string email, 
        string phone, 
        DateTime dateOfBirth,
        string password,
        List<Booking>? bookings = null,
        long id = 0) : base(name, email, phone, dateOfBirth, password, bookings, id )
    {
        Type = UserType.Manager;
    }

    public Manager()
    {
        
    }
    
    public Manager(User user) : base()
    {
        Id = user.Id;
        Name = user.Name;
        Type = UserType.Manager;
        Email = user.Email;
        Phone = user.Phone;
        DateOfBirth = user.DateOfBirth;
        Password = user.Password;
        Bookings = user.Bookings;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Manager manager
               && manager.Id == Id
               && manager.Name == Name
               && manager.Email == Email
               && manager.Phone == Phone
               && manager.DateOfBirth == DateOfBirth
               && manager.Password == Password
               && manager.Type == Type;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}