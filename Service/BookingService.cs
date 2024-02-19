using GarageProject.DAL;
using GarageProject.Models;
using GarageProject.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using GarageProject.Converters;
using Microsoft.AspNetCore.Http.HttpResults;
using GarageProject.Models.Enums;
using System.Globalization;

namespace GarageProject.Service
{
    public class BookingService : IBookingService
    {
        private readonly GarageProjectContext _context;
        private readonly IUserService _userService;
        private readonly IParkingSpaceService _parkingSpaceService;
        private readonly IDateTimeConverter _dateTimeConverter;

        public BookingService(
            GarageProjectContext context,
            IUserService userService,
            IParkingSpaceService parkingSpaceService,
            IDateTimeConverter dateTimeConverter
            )
        {
            _context = context;
            _userService = userService;
            _parkingSpaceService = parkingSpaceService;
            _dateTimeConverter = dateTimeConverter;
        }

        public async Task<bool> AddBooking( BookingDTO booking )
        {
            var user = await _userService.GetUserById( booking.UserId );
            if ( user == null )
            {
                throw new BadHttpRequestException( "Booking's user was not found" );
            }

            var startDateParsed = _dateTimeConverter.Convert( booking.Start );
            var endDateParsed = _dateTimeConverter.Convert( booking.End );

            return await AddBooking(user, startDateParsed, endDateParsed, booking.ParkingSpace );
        }

        public async Task<bool> AddBooking( User user, DateTime startDate, DateTime endDate, ParkingSpace? parkingSpace = null )
        {
            if ( parkingSpace == null || !await IsParkingSpaceFree( parkingSpace, startDate, endDate ) )
            {
                var availableParkingSpaces = await GetAvailableParkingSpacesForTimeRange( startDate, endDate );
                if ( availableParkingSpaces == null || availableParkingSpaces.Count() == 0 )
                {
                    throw new Exception( "There are no available parking spaces in the requested time range." );
                }
                parkingSpace = availableParkingSpaces.First();
            }

            Booking newBooking = new Booking( user, parkingSpace, startDate, endDate );
            await _context.Bookings.AddAsync( newBooking );
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Booking>?> GetAllBookings()
        {
            return await _context.Bookings
                .Include( b => b.User )
                .Include( b => b.ParkingSpace )
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingById( long id )
        {
            return await _context.Bookings.FirstOrDefaultAsync( b => b.Id == id );
        }
        public async Task<IEnumerable<Booking>?> GetBookingsByDates( string startDate, string endDate )
        {
            var startDateParsed = _dateTimeConverter.Convert( startDate );
            var endDateParsed = _dateTimeConverter.Convert( endDate );

            return await GetBookingsByDates( startDateParsed, endDateParsed );
        }

        public async Task<IEnumerable<Booking>?> GetBookingsByDates( DateTime startDate, DateTime endDate )
        {
            return await _context.Bookings.Where( b =>
            ( b.Start <= endDate && b.End >= startDate ) ||
            ( startDate <= b.End && endDate >= b.Start ) ||
            ( b.Start >= startDate && b.End <= endDate ) )
                .Include( b => b.User )
                .Include( b => b.ParkingSpace )
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>?> GetBookingsByUser( long userId )
        {
            var user = await _userService.GetUserById( userId );
            if ( user == null )
            {
                throw new BadHttpRequestException( $"User with id {userId} not found" );
            }

            return await _context.Bookings.Where( b => b.UserId == user.Id )
                .Include( b => b.User )
                .Include( b => b.ParkingSpace )
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>?> GetBookingsByUser( long userId, string startDate, string endDate )
        {
            var user = await _userService.GetUserById( userId );
            if ( user == null )
            {
                throw new BadHttpRequestException( $"User with id {userId} not found" );
            }

            var startDateParsed = _dateTimeConverter.Convert( startDate );
            var endDateParsed = _dateTimeConverter.Convert( endDate );

            return await GetBookingsByUser( user, startDateParsed, endDateParsed );
        }

        public async Task<IEnumerable<Booking>?> GetBookingsByUser( User user, DateTime startDate, DateTime endDate )
        {
            return await _context.Bookings.Where( b =>
            ( b.UserId == user.Id && b.Start <= endDate && b.End >= startDate ) ||
            ( b.UserId == user.Id && startDate <= b.End && endDate >= b.Start ) ||
            ( b.UserId == user.Id && b.Start >= startDate && b.End <= endDate ) )
                .Include( b => b.User )
                .Include( b => b.ParkingSpace )
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>?> GetListOfBookings( List<long> ids )
        {
            return await _context.Bookings.Where( b => ids.Contains( b.Id ) )
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

        public async Task<IEnumerable<ParkingSpace>?> GetAvailableParkingSpacesForTimeRange( DateTime startDate, DateTime endDate )
        {
            var parkingSpaces = await _parkingSpaceService.GetAllParkingSpaces();
            var bookingsForDates = await GetBookingsByDates( startDate, endDate );
            var listOfParkingSpacesInvolved = bookingsForDates == null ? null 
                : bookingsForDates.Select( b => b.ParkingSpace ).Distinct().ToList();

            return listOfParkingSpacesInvolved == null ?
                parkingSpaces :
                parkingSpaces?.Where( p => !listOfParkingSpacesInvolved.Contains( p ) ).ToList();
        }

        public async Task<int> GetNumberOfEmptySpacesForDate( string date )
        {
            var dateParsed = _dateTimeConverter.Convert( date );
            return await GetNumberOfEmptySpacesForDate( dateParsed );
        }

        public async Task<int> GetNumberOfEmptySpacesForDate( DateTime date )
        {
            var result = await GetAvailableParkingSpacesForDate( date );
            return result == null ? 0 : result.Count();
        }

        public async Task<IEnumerable<DateTime>> GetFullDaysOfMonth( string? date = null )
        {
            DateTime month;
            if( date == null )
            {
                month = DateTime.Now;
            } 
            else
            {
                month = _dateTimeConverter.Convert( date );
            }

            var result = new List<DateTime>();
            var daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);

            for(int i = 1; i <= daysInMonth; i++ )
            {
                var checkDay = new DateTime( month.Year, month.Month, i );
                var numberOfEmptySpaces = await GetNumberOfEmptySpacesForDate( checkDay );
                if ( numberOfEmptySpaces == 0 )
                {
                    result.Add( checkDay );
                }
            }
            return result;
        }

        public async Task<IEnumerable<ParkingSpace>?> GetAvailableParkingSpacesForDate( string date )
        {
            DateTime dateParsed;
            dateParsed = _dateTimeConverter.Convert( date );
            return await GetAvailableParkingSpacesForDate( dateParsed );
        }

        public async Task<IEnumerable<ParkingSpace>?> GetAvailableParkingSpacesForTimeRange( string startDate, string endDate )
        {
            var startDateParsed = _dateTimeConverter.Convert( startDate );
            var endDateParsed = _dateTimeConverter.Convert( endDate );

            return await GetAvailableParkingSpacesForTimeRange( startDateParsed, endDateParsed );
        }

        public async Task<bool> IsParkingSpaceFree( ParkingSpace space, DateTime start, DateTime end, long? bookingId = null )
        {
            var reassurance = await _parkingSpaceService.GetParkingSpaceById( space.Id );
            if ( reassurance == null )
            {
                throw new BadHttpRequestException( $"The provided parking space with id ${space.Id} was not found in the database." );
            }

            var bookings = await GetBookingsByDates( start, end );
            var bookingsWithSearchedSpace = bookings?.Where( b => b.ParkingSpace != null && b.ParkingSpace.Equals( space ) );

            if ( bookingsWithSearchedSpace != null && bookingsWithSearchedSpace.Count() > 0 )
            {
                //if it is the original booking which occupies the desired space
                if ( bookingId != null && bookingsWithSearchedSpace.Count() == 1 && bookingId == bookingsWithSearchedSpace.First().Id )
                {
                    return true;
                }
                return false;
            }
            return true;
        }

        public async Task<bool> RemoveDayFromBooking( long bookingId, string date, long userId )
        {
            var booking = await GetBookingById( bookingId );
            if ( booking == null )
            {
                throw new BadHttpRequestException( $"Booking with id {bookingId} not found." );
            }

            var day = _dateTimeConverter.Convert( date );

            if ( booking.Start.Date > day.Date || booking.End.Date < day.Date )
            {
                throw new InvalidOperationException( $"The booking with id {bookingId} does not include the day you provided ({day.Year}. {day.Month} {day.Day}). " +
                    $"Booking starts on {booking.Start.Year}. {booking.Start.Month} {booking.Start.Day}th at {booking.Start.Hour}:{booking.Start.Minute} and ends on" +
                    $"{booking.End.Year}. {booking.End.Month} {booking.End.Day}th at {booking.End.Hour}:{booking.End.Minute}" );
            }

            if ( booking.Start.Date == booking.End.Date && booking.Start.Date == day.Date )
            {
                await DeleteBooking( bookingId, userId );
                return true;
            }

            var modifiedDTO = new BookingDTO( booking );
            var newStart = day.AddDays( 1 );
            var newEnd = day.AddDays( -1 );
            newStart = new DateTime( newStart.Year, newStart.Month, newStart.Day, 0, 0, 0 );
            newEnd = new DateTime( newEnd.Year, newEnd.Month, newEnd.Day, 23, 59, 0 );
            if ( booking.Start.Date == day.Date )
            {
                modifiedDTO.Start = newStart.ToString( "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture );
                await UpdateBooking( bookingId, modifiedDTO, userId );
                return true;
            }

            if ( booking.End.Date == day.Date )
            {
                modifiedDTO.End = newEnd.ToString( "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture );
                await UpdateBooking( bookingId, modifiedDTO, userId );
                return true;
            }

            modifiedDTO.End = newEnd.ToString( "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture );
            await UpdateBooking( bookingId, modifiedDTO, userId );

            var newBooking = new BookingDTO( booking );
            newBooking.Start = newStart.ToString( "yyyy.MM.dd HH:mm", CultureInfo.InvariantCulture );
            await AddBooking( newBooking );
            return true;
        }

        public async Task<bool> FillDaysWithBookings(long loggedInUserId, long userId, string startDateString, string endDateString, ParkingSpace? parkingSpace = null )
        {
            var loggedInUser = await _userService.GetUserById( loggedInUserId );
            var user = loggedInUserId == userId ? loggedInUser : await _userService.GetUserById( userId );
            if( user == null || loggedInUser == null || ( user.Type != UserType.Manager && !loggedInUser.Equals(user) ) )
            {
                throw new UnauthorizedAccessException( "You are not authorized to update this booking." );
            }
            var startDate = _dateTimeConverter.Convert( startDateString );
            var endDate = _dateTimeConverter.Convert( endDateString );

            if(startDate > endDate)
            {
                throw new InvalidOperationException( "The start date must not be after the end date." );
            }

            var finished = false;
            var tasks = new List<Task>();

            while( !finished )
            {
                tasks.Add( AddBooking( user, startDate, endDate, parkingSpace ) );
                if( startDate.Date.Equals( endDate.Date ) )
                {
                    finished = true;
                }
                startDate = startDate.AddDays( 1 );
            }
            await Task.WhenAll( tasks );

            return true;
        }

        public async Task<bool> RemoveBookingsFromDaysInRange(long loggedInUserId, long userId, string startDateString, string endDateString )
        {
            var loggedInUser = await _userService.GetUserById( loggedInUserId );
            var user = loggedInUserId == userId ? loggedInUser : await _userService.GetUserById( userId );
            if ( user == null || loggedInUser == null || ( user.Type != UserType.Manager && !loggedInUser.Equals( user ) ) )
            {
                throw new UnauthorizedAccessException( "You are not authorized to update this booking." );
            }
            var startDate = _dateTimeConverter.Convert( startDateString );
            var endDate = _dateTimeConverter.Convert( endDateString );

            if ( startDate > endDate )
            {
                throw new InvalidOperationException( "The start date must not be after the end date." );
            }

            var bookings = await GetBookingsByUser(user, startDate, endDate);
            if( bookings == null )
            {
                throw new InvalidOperationException( $"No bookings were found within the provided date range for user {user.Name}." );
            }
            _context.RemoveRange( bookings );
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBooking( long id, BookingDTO newBooking, long userId )
        {
            var oldBooking = await GetBookingById( id );
            if ( oldBooking == null )
            {
                throw new BadHttpRequestException( $"Booking with id {id} not found." );
            }

            var user = await _userService.GetUserById( userId );
            bool canHandle = IsUserAuthorizedToHandleBooking( user, oldBooking );
            if ( !canHandle || user == null ) //method actually checks for null, this is only to suppress alert in IDE for possible null value issue
            {
                throw new UnauthorizedAccessException( "You are not authorized to update this booking." );
            }

            var startDateParsed = _dateTimeConverter.Convert( newBooking.Start );
            var endDateParsed = _dateTimeConverter.Convert( newBooking.End );

            ParkingSpace? parkingSpace;

            if ( newBooking.ParkingSpace != null && await IsParkingSpaceFree( newBooking.ParkingSpace, startDateParsed, endDateParsed, oldBooking.Id ) )
            {
                parkingSpace = newBooking.ParkingSpace;
            }
            else
            {
                //we find a suitable parking space
                var availableParkingSpaces = await GetAvailableParkingSpacesForTimeRange( startDateParsed, endDateParsed );
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

        public async Task<bool> DeleteBooking( long id, long userId )
        {
            var user = await _userService.GetUserById( userId );
            var booking = await GetBookingById( id );
            if ( booking == null )
            {
                throw new BadHttpRequestException( $"Booking with id {id} was not found." );
            }

            bool canHandle = IsUserAuthorizedToHandleBooking( user, booking );
            if ( !canHandle )
            {
                throw new UnauthorizedAccessException( "You are not authorized to update this booking." );
            }

            _context.Remove( booking );
            await _context.SaveChangesAsync();
            return true;
        }

        private bool IsUserAuthorizedToHandleBooking( User? user, Booking booking )
        {
            return user != null && ( booking.User.Id == user.Id || user.Type == UserType.Manager );
        }
    }
}
