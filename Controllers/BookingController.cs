using GarageProject.Converters;
using GarageProject.Models;
using GarageProject.Models.DTOs;
using GarageProject.Models.Enums;
using GarageProject.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GarageProject.Controllers
{
    [ApiController, Route( "booking" )]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IBookingConverter _converter;

        public BookingController(
            IBookingService bookingService,
            IBookingConverter converter
            )
        {
            _bookingService = bookingService;
            _converter = converter;
        }

        [HttpGet( "{id}" )]
        [Authorize]
        public async Task<IActionResult> GetBookingById( long id )
        {
            try
            {
                var query = await _bookingService.GetBookingById( id );
                var result = _converter.ConvertToBookingDTO( query );
                return Ok( result );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllBookings()
        {
            try
            {
                var query = await _bookingService.GetAllBookings();
                var result = _converter.ConvertToBookingDTOIEnumerable( query );
                return Ok( result );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpGet( "user/{userId}" )]
        [Authorize]
        public async Task<IActionResult> GetBookingsByUser( long userId )
        {
            try
            {
                var query = await _bookingService.GetBookingsByUser( userId );
                var result = _converter.ConvertToBookingDTOIEnumerable( query );
                return Ok( result );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpGet( "dates" )]
        [Authorize]
        public async Task<IActionResult> GetBookingsByDates(
        [FromQuery] string startDate,
        [FromQuery] string endDate )
        {
            try
            {
                var query = await _bookingService.GetBookingsByDates( startDate, endDate );
                var result = _converter.ConvertToBookingDTOIEnumerable( query );
                return Ok( result );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpGet( "user/dates" )]
        [Authorize]
        public async Task<IActionResult> GetBookingsByUserAndDates(
        [FromQuery] long userId,
        [FromQuery] string startDate,
        [FromQuery] string endDate )
        {
            try
            {
                var query = await _bookingService.GetBookingsByUser( userId, startDate, endDate );
                var result = _converter.ConvertToBookingDTOIEnumerable( query );
                return Ok( result );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }

        }

        [HttpGet( "ids" )]
        [Authorize]
        public async Task<IActionResult> GetBookingsByListOfIds( [FromBody] List<long> ids )
        {
            try
            {
                var query = await _bookingService.GetListOfBookings( ids );
                var result = _converter.ConvertToBookingDTOIEnumerable( query );
                return Ok( result );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }

        }

        [HttpGet( "emptyspaces/date/{date}" )]
        [Authorize]
        public async Task<IActionResult> GetEmptyParkingSpaces( string date )
        {
            try
            {
                var result = await _bookingService.GetAvailableParkingSpacesForDate( date );
                return Ok( result );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpGet( "emptyspaces/daterange" )]
        [Authorize]
        public async Task<IActionResult> GetEmptyParkingSpacesForTimeRange(
        [FromQuery] string startDate,
        [FromQuery] string endDate )
        {
            try
            {
                var result = await _bookingService.GetAvailableParkingSpacesForTimeRange( startDate, endDate );
                return Ok( result );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpGet( "emptyspaces/amount/{date}" )]
        [Authorize]
        public async Task<IActionResult> GetAmountOfEmptySpacesForDate( string date )
        {
            try
            {
                var result = await _bookingService.GetNumberOfEmptySpacesForDate( date );
                return Ok( result );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpGet("fulldaysofmonth/{date}")]
        [Authorize]
        public async Task<IActionResult> GetFullDaysOfMonth( string date )
        {
            try
            {
                var result = await _bookingService.GetFullDaysOfMonth( date );
                return Ok( result );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpPost("filldays")]
        [Authorize]
        public async Task<IActionResult> FillDaysWithBookings(
        [FromBody] long userId,
        [FromBody] string startDate,
        [FromBody] string endDate,
        [FromBody] ParkingSpace parkingSpace )
        {
            try
            {
                var loggedInUserId = GetLoggedInUserId();
                var result = await _bookingService.FillDaysWithBookings(loggedInUserId, userId, startDate, endDate, parkingSpace );
                return Ok( $"Filling dates between {startDate} and {endDate} was successful." );
            }
            catch( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpPost("removedays")]
        [Authorize]
        public async Task<IActionResult> RemoveBookingsFromDaysInRange(
        [FromQuery] long userId,
        [FromQuery] string startDate,
        [FromQuery] string endDate )
        {
            try
            {
                var loggedInUserId = GetLoggedInUserId();
                var result = await _bookingService.RemoveBookingsFromDaysInRange( loggedInUserId, userId, startDate, endDate );
                return Ok( $"Removing bookings from dates between {startDate} and {endDate} was successful." );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddBooking( [FromBody] BookingDTO booking )
        {
            try
            {
                var result = await _bookingService.AddBooking( booking );
                if ( result )
                {
                    return Ok( "Adding booking was successful." );
                }
                else
                {
                    return BadRequest( "Adding booking failed." );
                }
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpPut("removeday/{id}/{date}")]
        [Authorize]
        public async Task<IActionResult> RemoveDayFromBooking( long id, string date )
        {
            try
            {
                var userId = GetLoggedInUserId();
                var result = await _bookingService.RemoveDayFromBooking( id, date, userId );
                if( result )
                {
                    return Ok( "Removing day from booking was successful." );
                } else
                {
                    return BadRequest( "Removing day from booking failed." );
                }
            }
            catch (Exception ex)
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpPut( "{id}" )]
        [Authorize]
        public async Task<IActionResult> UpdateBooking( long id, [FromBody] BookingDTO newBooking )
        {
            try
            {
                var userId = GetLoggedInUserId();
                var result = await _bookingService.UpdateBooking( id, newBooking, userId );
                if ( result )
                {
                    return Ok( "Updating booking was successful." );
                }
                else
                {
                    return BadRequest( "Updating booking failed." );
                }
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpDelete( "{id}" )]
        [Authorize]
        public async Task<IActionResult> DeleteBooking( long id )
        {
            try
            {
                var userId = GetLoggedInUserId();
                var result = await _bookingService.DeleteBooking( id, userId );
                if ( result )
                {
                    return Ok( "Booking deletion was successful." );
                }
                else
                {
                    return BadRequest( "Booking deletion failed." );
                }
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        private long GetLoggedInUserId()
        {
            long userId;
            long.TryParse( HttpContext?.User?.Claims?.FirstOrDefault( claim => claim.Type == ClaimTypes.Authentication )?.Value, out userId );
            return userId;
        }
    }
}
