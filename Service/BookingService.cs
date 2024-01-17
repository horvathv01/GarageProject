using GarageProject.DAL;
using GarageProject.Models;
using GarageProject.Models.DTOs;
using GarageProject.Service;
using Microsoft.EntityFrameworkCore;
using GarageProject.Converters;
using GarageProject.Models;
using System.ComponentModel;

namespace GarageProject.Service
{
    public class BookingService : IBookingService
    {
        private readonly GarageProjectContext _context;
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;
        

        public BookingService( GarageProjectContext context, IServiceProvider serviceProvider, IUserService userService ) 
        { 
            _context = context; 
            _serviceProvider = serviceProvider;
            _userService = userService;
        }

        public async Task<bool> AddBooking( BookingDTO booking )
        {
            try
            {
                var user = await _userService.GetUserById( booking.User.Id );

                if(user == null )
                {
                    throw new Exception("Booking's user was not found");
                }

                var parkingSpaceService = _serviceProvider.GetService<IParkingSpaceService>();
                var dateTimeConverter = _serviceProvider.GetService<IDateTimeConverter>();
                if ( parkingSpaceService == null || dateTimeConverter == null )
                {
                    throw new Exception( "Dependency injection failed." );
                }

                var parkingSpace = await parkingSpaceService.GetParkingSpaceById( booking.ParkingSpace.Id );

                if ( parkingSpace == null )
                {
                    throw new Exception( "The parking space the booking refers to was not found" );
                }

                var startDateParsed = dateTimeConverter.Convert(booking.Start);
                var endDateParsed = dateTimeConverter.Convert( booking.End );

                Booking newBooking = new Booking(user, parkingSpace, startDateParsed, endDateParsed);
                await _context.Bookings.AddAsync( newBooking );
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<IEnumerable<Booking>?> GetAllBookings()
        {
            return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.ParkingSpace)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingById( long id )
        {
            return await _context.Bookings.FirstOrDefaultAsync( b => b.Id == id );
        }

        public async Task<IEnumerable<Booking>?> GetBookingsByDates( string startDate, string endDate )
        {
            var dateTimeConverter = _serviceProvider.GetService<IDateTimeConverter>();
            if ( dateTimeConverter == null )
            {
                throw new Exception( "Dependency injection failed." );
            }
            var startDateParsed = dateTimeConverter.Convert( startDate );
            var endDateParsed = dateTimeConverter.Convert( endDate );

            return await _context.Bookings.Where( b => b.Start >= startDateParsed && b.End <= endDateParsed )
                .Include( b => b.User )
                .Include( b => b.ParkingSpace )
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>?> GetBookingsByUser( User user )
        {
            return await _context.Bookings.Where( b => b.UserId == user.Id )
                .Include( b => b.User )
                .Include( b => b.ParkingSpace )
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>?> GetBookingsByUser( User user, string startDate, string endDate )
        {
            var dateTimeConverter = _serviceProvider.GetService<IDateTimeConverter>();
            if ( dateTimeConverter == null )
            {
                throw new Exception( "Dependency injection failed." );
            }
            var startDateParsed = dateTimeConverter.Convert( startDate );
            var endDateParsed = dateTimeConverter.Convert( endDate );

            return await _context.Bookings.Where( b => b.UserId == user.Id && b.Start >= startDateParsed && b.End <= endDateParsed )
                .Include( b => b.User )
                .Include( b => b.ParkingSpace )
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>?> GetListOfBookings( List<long> ids )
        {
            return await _context.Bookings.Where(b => ids.Contains(b.Id ) )
                .Include( b => b.User )
                .Include( b => b.ParkingSpace )
                .ToListAsync();
        }

        public async Task<bool> UpdateBooking( Booking oldBooking, BookingDTO newBooking )
        {
            try
            {
                var parkingSpaceService = _serviceProvider.GetService<IParkingSpaceService>();
                var dateTimeConverter = _serviceProvider.GetService<IDateTimeConverter>();
                if ( parkingSpaceService == null || dateTimeConverter == null )
                {
                    throw new Exception( "Dependency injection failed." );
                }

                var parkingSpace = await parkingSpaceService.GetParkingSpaceById( newBooking.ParkingSpace.Id );

                if ( parkingSpace == null )
                {
                    throw new Exception( "The parking space the booking refers to was not found" );
                }

                var startDateParsed = dateTimeConverter.Convert( newBooking.Start );
                var endDateParsed = dateTimeConverter.Convert( newBooking.End );

                var user = await _userService.GetUserById( newBooking.User.Id );
                if ( user == null )
                {
                    throw new Exception( "Booking's user was not found" );
                }

                oldBooking.User = user;
                oldBooking.ParkingSpace = parkingSpace;
                oldBooking.Start = startDateParsed;
                oldBooking.End = endDateParsed;
                
                _context.Update( oldBooking );
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteBooking( Booking booking )
        {
            try
            {
                _context.Remove( booking );
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
