using GarageProject.Models.Enums;

namespace GarageProject.Models;

public class UserDTO
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string DateOfBirth { get; set; }
    public string Password { get; set; }
    

    [Newtonsoft.Json.JsonConstructor]
    public UserDTO(
        long id, 
        string name, 
        string type, 
        string email, 
        string phone, 
        string dateOfBirth, 
        string password
            )
    {
        Id = id;
        Name = name;
        Type = type;
        Email = email;
        Phone = phone;
        DateOfBirth = dateOfBirth; 
        Password = password;
    }

    public UserDTO(User user)
    {
        Id = user.Id;
        Name = user.Name;
        Type =  user.Type.ToString();
        Email = user.Email;
        Phone = user.Phone;
        DateOfBirth = user.DateOfBirth.ToString();
        Password = user.Password;
    }

    public override bool Equals( object? obj )
    {
        return obj is UserDTO user
            && user.Id.Equals( Id )
            && user.Name.Equals( Name )
            && user.Type.Equals( Type )
            && user.Email.Equals( Email )
            && user.Phone.Equals( Phone )
            && user.DateOfBirth == DateOfBirth;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine( Id, Name, Type, Email, Phone, DateOfBirth, Password );
    }
}