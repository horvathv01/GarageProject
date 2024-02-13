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
        long id = 0) : base( name, email, phone, dateOfBirth, password, bookings, id )
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

    public override bool Equals( object? obj )
    {
        return obj is Manager manager &&
                base.Equals( obj ) &&
                 Id == manager.Id &&
                 Name == manager.Name &&
                 Type == manager.Type &&
                 Email == manager.Email &&
                 Phone == manager.Phone &&
                 DateOfBirth == manager.DateOfBirth &&
                 Password == manager.Password &&
                EqualityComparer<List<Booking>>.Default.Equals( Bookings, manager.Bookings );
    }

    public override int GetHashCode()
    {
        return HashCode.Combine( Id, Name, Type, Email, Phone, DateOfBirth, Password, Bookings );
    }
}