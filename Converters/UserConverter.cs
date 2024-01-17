using GarageProject.Models;
using GarageProject.Models.DTOs;

namespace PsychAppointments_API.Converters
{
    public class UserConverter : IUserConverter
    {
        public UserDTO? ConvertToUserDTO( User? user )
        {
            return user == null ? null : new UserDTO( user );
        }

        public IEnumerable<UserDTO>? ConvertToUserDTOIEnumerable( IEnumerable<User>? users )
        {
            return users?.Select( b => new UserDTO( b ) ).ToList() ?? new List<UserDTO>();
        }
    }
}
