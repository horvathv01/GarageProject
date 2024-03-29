﻿using GarageProject.DAL;
using Microsoft.EntityFrameworkCore;
using GarageProject.Models;
using GarageProject.Models.Enums;

namespace GarageProject.Service
{
    public class ParkingSpaceService : IParkingSpaceService
    {
        private readonly GarageProjectContext _context;
        private readonly IUserService _userService;

        public ParkingSpaceService( GarageProjectContext context, IUserService userService )
        {
            _context = context;
            _userService = userService;
        }

        public async Task<bool> AddParkingSpace( ParkingSpace space )
        {
                await _context.ParkingSpaces.AddAsync( space );
                await _context.SaveChangesAsync();
                return true;
        }

        public async Task<IEnumerable<ParkingSpace>?> GetAllParkingSpaces()
        {
            return await _context.ParkingSpaces.ToListAsync();
        }

        public async Task<ParkingSpace?> GetParkingSpaceById( long id )
        {
            return await _context.ParkingSpaces.FirstOrDefaultAsync( s => s.Id == id );
        }

        public async Task<IEnumerable<ParkingSpace>?> GetListOfParkingSpaces( List<long> ids )
        {
            return await _context.ParkingSpaces.Where( s => ids.Contains( s.Id ) ).ToListAsync();
        }

        public async Task<bool> UpdateParkingSpace( long id, ParkingSpace newSpace, long userId )
        {
            var user = await _userService.GetUserById( userId );

            if ( user == null || user.Type != UserType.Manager )
            {
                throw new InvalidOperationException( "You are not authorized to update this parking space." );
            }

            var space = await GetParkingSpaceById( id );
            if ( space != null )
            {
                space.IsDeleted = newSpace.IsDeleted;
                _context.ParkingSpaces.Update( space );
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception( $"Parking space with id {id} not found in db." );
            }
        }

        public async Task<bool> DeleteParkingSpace( long id, long userId )
        {
            var user = await _userService.GetUserById( userId );

            if ( user == null || user.Type != UserType.Manager )
            {
                throw new InvalidOperationException( "You are not authorized to delete this parking space." );
            }

            var space = await GetParkingSpaceById( id );
            //DO NOT DELETE! set parkingSpace.IsDeleted to true!
            if ( space != null )
            {
                space.IsDeleted = true;
                _context.ParkingSpaces.Update( space );
                await _context.SaveChangesAsync();
                return true;
            }
            else
            {
                throw new Exception( $"Parking space with id {id} not found in db." );
            }
        }
    }
}
