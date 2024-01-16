using GarageProject.Models;
using GarageProject.Models.DTOs;
using GarageProject.Models.Enums;
using GarageProject.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PsychAppointments_API.Models;
using PsychAppointments_API.Service;
using System.Security.Claims;

namespace PsychAppointments_API.Controllers
{
    [ApiController, Route( "booking" )]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IUserService _userService;

        public BookingController( IBookingService bookingService, IUserService userService ) 
        {
            _bookingService = bookingService;
            _userService = userService;
        }

        [HttpGet("{id}")]
        //[Authorize]
        public async Task<BookingDTO?> GetBookingById(long id )
        {
            var result = await _bookingService.GetBookingById( id );
            if(result == null )
            {
                return new BookingDTO( result );
            }
            return null;
        }

        [HttpGet()]
        //[Authorize]
        public async Task<IEnumerable<BookingDTO>?> GetAllBookings()
        {
            var result = await _bookingService.GetAllBookings();
            return result?.Select( b => new BookingDTO( b ) ).ToList();
        }

        [HttpGet("user/{userId}")]
        //[Authorize]
        public async Task<IEnumerable<BookingDTO>?> GetBookingsByUser(long userId)
        {
            var user = await _userService.GetUserById( userId );
            if ( user == null )
            {
                throw new Exception( $"User with id {userId} not found" );
            }
            //should we allow this to managers only?
            var result = await _bookingService.GetBookingsByUser( user );
            return result?.Select( b => new BookingDTO( b ) ).ToList();
        }

        [HttpGet( "user/dates" )]
        //[Authorize]
        public async Task<IEnumerable<BookingDTO>?> GetBookingsByDates(
        [FromQuery] string startDate,
        [FromQuery] string endDate )
        {
            var result = await _bookingService.GetBookingsByDates( startDate, endDate );
            return result?.Select( b => new BookingDTO( b ) ).ToList();
        }

        [HttpGet("user/dates")]
        //[Authorize]
        public async Task<IEnumerable<BookingDTO>?> GetBookingsByUserAndDates( 
        [FromQuery] long userId,
        [FromQuery] string startDate,
        [FromQuery] string endDate )
        {
            var user = await _userService.GetUserById( userId );
            if( user == null )
            {
                throw new Exception( $"User with id {userId} not found" );
            }
            var result = await _bookingService.GetBookingsByUser( user, startDate, endDate );
            return result?.Select(b => new BookingDTO(b)).ToList();
        }

        [HttpGet("ids")]
        //[Authorize]
        public async Task<IEnumerable<BookingDTO>?> GetBookingsByListOfIds([FromBody] List<long> ids)
        {
            
            var result = await _bookingService.GetListOfBookings( ids );
            return result?.Select(b => new BookingDTO(b)).ToList();
        }

        [HttpPost()]
        //[Authorize]
        public async Task<IActionResult> AddBooking(BookingDTO booking )
        {
            var result = await _bookingService.AddBooking(booking);
            if ( result )
            {
                return Ok("Adding booking was successful.");
            }
            return BadRequest("Something went wrong.");
        }

        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> UpdateBooking(long id, [FromBody] BookingDTO newBooking)
        {
            var booking = await _bookingService.GetBookingById( id );
            if ( booking == null )
            {
                return BadRequest( $"Booking with id {id} not found." );
            }

            var user = await GetLoggedInUser();
            //only allow if user is manager or booking belongs to user
            bool canHandle = IsUserAuthorizedToHandleBooking( user, booking );
            if ( !canHandle )
            {
                return Unauthorized( "You are not authorized to update this booking." );
            }

            var result = await _bookingService.UpdateBooking( booking, newBooking );
            if ( result )
            {
                return Ok( "Updating booking was successful." );
            }
            else
            {
                return BadRequest( "Updating booking failed." );
            }
        }

        [HttpDelete("{id}")]
        //[Authorize]
        public async Task<IActionResult> DeleteBooking(long id)
        {
            var booking = await _bookingService.GetBookingById(id);
            if (booking == null)
            {
                return BadRequest($"Booking with id {id} not found.");
            }

            var user = await GetLoggedInUser();
            //only allow if user is manager or booking belongs to user
            bool canHandle = IsUserAuthorizedToHandleBooking(user, booking);
            if ( !canHandle )
            {
                return Unauthorized("You are not authorized to delete this booking.");
            }
            
            var result = await _bookingService.DeleteBooking( booking );
            if ( result )
            {
                return Ok("Booking deletion was successful.");
            }
            else
            {
                return BadRequest( "Booking deletion failed." );
            }
        }

        private async Task<User?> GetLoggedInUser()
        {
            long userId;
            long.TryParse( HttpContext?.User?.Claims?.FirstOrDefault( claim => claim.Type == ClaimTypes.Authentication )?.Value, out userId );
            return await _userService.GetUserById( userId );
        }

        private bool IsUserAuthorizedToHandleBooking( User? user, Booking booking )
        {
            return user != null && ( booking.UserId == user.Id || user.Type == UserType.Manager );
        }


    }
}
