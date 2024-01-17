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
        public async Task<ParkingSpace?> GetParkingSpaceById( long id )
        {
            return await _parkingSpaceService.GetParkingSpaceById( id );
        }

        [HttpGet]
        //[Authorize]
        public async Task<IEnumerable<ParkingSpace>?> GetAllParkingSpaces( long id )
        {
            return await _parkingSpaceService.GetAllParkingSpaces();
        }

        [HttpGet( "ids" )]
        //[Authorize]
        public async Task<IEnumerable<ParkingSpace>?> GetParkingSpacesByListOfIds( [FromBody] List<long> ids )
        {
            return await _parkingSpaceService.GetListOfParkingSpaces( ids );
        }

        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> AddParkingSpace( [FromBody] ParkingSpace space )
        {
            var result = await _parkingSpaceService.AddParkingSpace( space );
            if ( result )
            {
                return Ok( "Adding parking space was successful." );
            }
            return BadRequest( "Something went wrong." );
        }

        [HttpPut( "{id}" )]
        //[Authorize]
        public async Task<IActionResult> UpdateParkingSpace( long id, [FromBody] ParkingSpace newSpace)
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

        [HttpDelete( "{id}" )]
        //[Authorize]
        public async Task<IActionResult> DeleteParkingSpace( long id )
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

        private async Task<User?> GetLoggedInUser()
        {
            long userId;
            long.TryParse( HttpContext?.User?.Claims?.FirstOrDefault( claim => claim.Type == ClaimTypes.Authentication )?.Value, out userId );
            return await _userService.GetUserById( userId );
        }
    }
}
