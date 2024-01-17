using GarageProject.DAL;
using GarageProject.Models;
using GarageProject.Models.DTOs;
using GarageProject.Service;
using Microsoft.EntityFrameworkCore;
using GarageProject.Converters;
using System.ComponentModel;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

                var startDateParsed = dateTimeConverter.Convert(booking.Start);
                var endDateParsed = dateTimeConverter.Convert( booking.End );

                var availableParkingSpaces = await GetAvailableParkingSpacesForTimeRange(startDateParsed, endDateParsed);
                if( availableParkingSpaces == null || availableParkingSpaces.Count() == 0 ) 
                {
                    throw new Exception( "There is no available parking space in the requested time range." );
                }

                Booking newBooking = new Booking(user, availableParkingSpaces.First(), startDateParsed, endDateParsed);
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

            return await GetBookingsByDates(startDateParsed, endDateParsed );
        }

        public async Task<IEnumerable<Booking>?> GetBookingsByDates( DateTime startDate, DateTime endDate )
        {
            return await _context.Bookings.Where(b =>
            ( b.Start <= endDate && b.End >= startDate ) ||
            ( startDate <= b.End && endDate >= b.Start ) ||
            ( b.Start >= startDate && b.End <= endDate ))
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

            return await GetBookingsByUser(user, startDateParsed, endDateParsed );
        }

        public async Task<IEnumerable<Booking>?> GetBookingsByUser( User user, DateTime startDate, DateTime endDate )
        {
            return await _context.Bookings.Where( b =>
            ( b.UserId == user.Id && b.Start <= endDate && b.End >= startDate ) ||
            ( b.UserId == user.Id && startDate <= b.End && endDate >= b.Start ) ||
            ( b.UserId == user.Id && b.Start >= startDate && b.End <= endDate ))
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

        public async Task<IEnumerable<ParkingSpace>?> GetAvailableParkingSpacesForDate( DateTime date )
        {
            var startDate = new DateTime( date.Year, date.Month, date.Day, 0, 0, 0 );
            var endDate = new DateTime( date.Year, date.Month, date.Day, 23, 59, 59 );
            return await GetAvailableParkingSpacesForTimeRange( startDate, endDate );
        }

        public async Task<IEnumerable<ParkingSpace>?> GetAvailableParkingSpacesForTimeRange(DateTime startDate, DateTime endDate )
        {
            var parkingSpaceService = _serviceProvider.GetService<IParkingSpaceService>();
            if ( parkingSpaceService == null )
            {
                throw new Exception( "Dependency injection failed." );
            }
            var parkingSpaces = await parkingSpaceService.GetAllParkingSpaces();
            var bookingsForDates = await GetBookingsByDates( startDate, endDate );
            var listOfParkingSpacesInvolved = bookingsForDates?.Select( b => b.ParkingSpace ).Distinct().ToList();

            return listOfParkingSpacesInvolved == null ?
                parkingSpaces :
                parkingSpaces?.Where( p => !listOfParkingSpacesInvolved.Contains( p ) ).ToList();
        }

        public async Task<int> GetNumberOfEmptySpacesForDate( string date )
        {
            var dateTimeConverter = _serviceProvider.GetService<IDateTimeConverter>();
            if ( dateTimeConverter == null )
            {
                throw new Exception( "Dependency injection failed." );
            }
            var dateParsed = dateTimeConverter.Convert( date );
            return await GetNumberOfEmptySpacesForDate( dateParsed );
        }

        public async Task<int> GetNumberOfEmptySpacesForDate( DateTime date )
        {
            var result = await GetAvailableParkingSpacesForDate( date );
            return result == null ? 0 : result.Count();
        }

        public async Task<bool> IsParkingSpaceFree(ParkingSpace space, DateTime start, DateTime end, long? bookingId = null)
        {
            var bookings = await GetBookingsByDates( start, end );
            var bookingsWithSearchedSpace = bookings?.Where( b => b.ParkingSpace.Equals( space ) );

            if( bookingsWithSearchedSpace != null && bookingsWithSearchedSpace.Count() > 0 )
            {
                //if it is the original booking which occupies the desired space
                if( bookingId != null && bookingsWithSearchedSpace.Count() == 1 && bookingId == bookingsWithSearchedSpace.First().Id )
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        public async Task<bool> UpdateBooking( Booking oldBooking, BookingDTO newBooking )
        {
            try
            {
                var user = await _userService.GetUserById( newBooking.User.Id );
                if ( user == null )
                {
                    throw new Exception( "Booking's user was not found" );
                }

                var parkingSpaceService = _serviceProvider.GetService<IParkingSpaceService>();
                var dateTimeConverter = _serviceProvider.GetService<IDateTimeConverter>();
                if ( parkingSpaceService == null || dateTimeConverter == null )
                {
                    throw new Exception( "Dependency injection failed." );
                }

                var startDateParsed = dateTimeConverter.Convert( newBooking.Start );
                var endDateParsed = dateTimeConverter.Convert( newBooking.End );

                ParkingSpace? parkingSpace;
                
                if( newBooking.ParkingSpace != null && await IsParkingSpaceFree( newBooking.ParkingSpace, startDateParsed, endDateParsed, oldBooking.Id ) )
                {
                    parkingSpace = newBooking.ParkingSpace;
                }
                else
                {
                    //we find a suitable parking space
                    var availableParkingSpaces = await GetAvailableParkingSpacesForTimeRange(startDateParsed, endDateParsed);
                    if ( availableParkingSpaces == null || availableParkingSpaces.Count() == 0 )
                    {
                        throw new Exception( "There are no available parking spaces in the requested time range." );
                    }
                    parkingSpace = availableParkingSpaces.First();
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
