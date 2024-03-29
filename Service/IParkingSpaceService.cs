﻿using GarageProject.Models;
using GarageProject.Models;

namespace GarageProject.Service
{
    public interface IParkingSpaceService
    {
        Task<bool> AddParkingSpace( ParkingSpace space );
        Task<ParkingSpace?> GetParkingSpaceById( long id );
        Task<IEnumerable<ParkingSpace>?> GetAllParkingSpaces();
        Task<IEnumerable<ParkingSpace>?> GetListOfParkingSpaces( List<long> ids );
        Task<bool> UpdateParkingSpace( long id, ParkingSpace space, long userId );
        Task<bool> DeleteParkingSpace( long id, long userId );
    }
}
