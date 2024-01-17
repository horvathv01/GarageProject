using System.ComponentModel.DataAnnotations.Schema;
using GarageProject.Models.Enums;
using GarageProject.Models;

namespace GarageProject.Models;

public class User
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    public string Name { get; set; }
    public UserType Type { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Password { get; set; }

    public List<Booking> Bookings { get; set; }

    public User(string name, string email, string phone, DateTime dateOfBirth, string password, List<Booking>? bookings = null, long id = 0)
    {
        Name = name;
        Email = email;
        Phone = phone;
        DateOfBirth = dateOfBirth;
        Password = password;
        Bookings = bookings == null ? new List<Booking>() : bookings;
        Id = id;
    }

    public User()
    {
        
    }

    public override bool Equals(object? obj)
    {
        return obj is User user
               && user.Name == Name
               && user.Email == Email
               && user.Phone == Phone
               && user.DateOfBirth == DateOfBirth
               && user.Password == Password
               && user.Id == Id
               && user.Type == Type;
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}