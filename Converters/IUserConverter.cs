using GarageProject.Models;

namespace PsychAppointments_API.Converters
{
    public interface IUserConverter
    {
        UserDTO? ConvertToUserDTO( User? user );
        IEnumerable<UserDTO>? ConvertToUserDTOIEnumerable( IEnumerable<User>? users );
    }
}
