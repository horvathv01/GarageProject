using GarageProject.Models;
using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service
{
    public interface IParkingSpaceService
    {
        Task<bool> AddParkingSpace( ParkingSpace space );
        Task<ParkingSpace?> GetParkingSpaceById( long id );
        Task<IEnumerable<ParkingSpace>?> GetAllParkingSpaces();
        Task<IEnumerable<ParkingSpace>?> GetListOfParkingSpaces( List<long> ids );
        Task<bool> UpdateParkingSpace( long id, ParkingSpace space );
        Task<bool> DeleteParkingSpace( long id );
    }
}
