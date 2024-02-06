using GarageProject.Models;

namespace GarageProject.Converters
{
    public interface IUserConverter
    {
        UserDTO? ConvertToUserDTO( User? user );
        IEnumerable<UserDTO>? ConvertToUserDTOIEnumerable( IEnumerable<User>? users );
    }
}
