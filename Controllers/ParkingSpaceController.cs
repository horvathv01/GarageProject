using GarageProject.Models;
using GarageProject.Models.DTOs;
using GarageProject.Models.Enums;
using GarageProject.Service;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PsychAppointments_API.Controllers
{
    [ApiController, Route( "spaces" )]
    public class ParkingSpaceController : ControllerBase
    {
        private readonly IParkingSpaceService _parkingSpaceService;
        private readonly IUserService _userService;

        public ParkingSpaceController( IParkingSpaceService parkingSpaceService, IUserService userService )
        {
            _parkingSpaceService = parkingSpaceService;
            _userService = userService;
        }

        [HttpGet( "{id}" )]
        //[Authorize]
        public async Task<IActionResult> GetParkingSpaceById( long id )
        {
            try
            {
                var result = await _parkingSpaceService.GetParkingSpaceById( id );
                return Ok( result );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpGet]
        //[Authorize]
        public async Task<IActionResult> GetAllParkingSpaces( long id )
        {
            try
            {
                var result = await _parkingSpaceService.GetAllParkingSpaces();
                return Ok( result );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpGet( "ids" )]
        //[Authorize]
        public async Task<IActionResult> GetParkingSpacesByListOfIds( [FromBody] List<long> ids )
        {
            try
            {
                var result = await _parkingSpaceService.GetListOfParkingSpaces( ids );
                return Ok( result );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> AddParkingSpace( [FromBody] ParkingSpace space )
        {
            try
            {
                var result = await _parkingSpaceService.AddParkingSpace( space );
                if ( result )
                {
                    return Ok( "Adding parking space was successful." );
                }
                return BadRequest( "Something went wrong." );
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpPut( "{id}" )]
        //[Authorize]
        public async Task<IActionResult> UpdateParkingSpace( long id, [FromBody] ParkingSpace newSpace )
        {
            try
            {
                var user = await GetLoggedInUser();
                var result = await _parkingSpaceService.UpdateParkingSpace( id, newSpace, user );
                if ( result )
                {
                    return Ok( "Updating parking space was successful." );
                }
                else
                {
                    return BadRequest( "Updating parking space failed." );
                }
            }
            catch ( Exception ex )
            {
                return BadRequest( ex.Message );
            }
        }

        [HttpDelete( "{id}" )]
        //[Authorize]
        public async Task<IActionResult> DeleteParkingSpace( long id )
        {
            try
            {
                var user = await GetLoggedInUser();
                var result = await _parkingSpaceService.DeleteParkingSpace( id, user );
                if ( result )
                {
                    return Ok( "Parking space deletion was successful." );
                }
                else
                {
                    return BadRequest( "Parking space deletion failed." );
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
