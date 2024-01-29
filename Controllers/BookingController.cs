using GarageProject.Converters;
using GarageProject.Models;
using GarageProject.Models.DTOs;
using GarageProject.Models.Enums;
using GarageProject.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychAppointments_API.Converters;
using System.Security.Claims;

namespace GarageProject.Controllers
{
    [ApiController, Route( "booking" )]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;
        private readonly IBookingConverter _converter;

        public BookingController(
            IBookingService bookingService,
            IUserService userService,
            IBookingConverter converter
            )
        {
            _bookingService = bookingService;
            _userService = userService;
            _converter = converter;
        }

        [HttpGet( "{id}" )]
        //[Authorize]
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
        //[Authorize]
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
        //[Authorize]
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
        //[Authorize]
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
        //[Authorize]
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
        //[Authorize]
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
        //[Authorize]
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
        //[Authorize]
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
        //[Authorize]
        public async Task<IActionResult> GetAmountOfEmptySpacesFoRDate( string date )
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


        [HttpPost]
        //[Authorize]
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

        [HttpPut( "{id}" )]
        //[Authorize]
        public async Task<IActionResult> UpdateBooking( long id, [FromBody] BookingDTO newBooking )
        {
            try
            {
                var user = await GetLoggedInUser();
                var result = await _bookingService.UpdateBooking( id, newBooking, user );
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
        //[Authorize]
        public async Task<IActionResult> DeleteBooking( long id )
        {
            try
            {
                var user = await GetLoggedInUser();
                var result = await _bookingService.DeleteBooking( id, user );
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

        private async Task<User?> GetLoggedInUser()
        {
            long userId;
            long.TryParse( HttpContext?.User?.Claims?.FirstOrDefault( claim => claim.Type == ClaimTypes.Authentication )?.Value, out userId );
            return await _userService.GetUserById( userId );
        }
    }
}
