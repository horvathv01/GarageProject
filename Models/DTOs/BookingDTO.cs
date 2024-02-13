using GarageProject.Models;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace GarageProject.Models.DTOs
{
    public class BookingDTO
    {
        public long Id { get; set; }

        public long UserId { get; set; }

        public ParkingSpace? ParkingSpace { get; set; }

        public string Start { get; set; }
        public string End { get; set; }


        [JsonConstructor]
        public BookingDTO( long userId, string start, string end, ParkingSpace? parkingSpace = null, long id = 0 )
        {
            UserId = userId;
            ParkingSpace = parkingSpace;
            Start = start;
            End = end;
            Id = id;
        }

        public BookingDTO()
        {

        }

        public BookingDTO( Booking booking )
        {
            Id = booking.Id;
            UserId = booking.UserId;
            ParkingSpace = booking.ParkingSpace;
            Start = booking.Start.ToString( "yyyy\\-MM\\-dd\\H\\-mm\\-ss" ); //or maybe "yyyy-MM-dd-HH-mm-ss"
            End = booking.End.ToString( "yyyy\\-MM\\-dd\\H\\-mm\\-ss" );
        }

        public override bool Equals( object? obj )
        {
            return obj is BookingDTO dto
                && dto.Id.Equals( Id )
                && dto.UserId == UserId
                && ( dto.ParkingSpace == null ? ParkingSpace == null : dto.ParkingSpace.Equals(ParkingSpace) )
                && dto.Start.Equals( Start )
                && dto.End.Equals( End );
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( Id, UserId, ParkingSpace, Start, End );
        }
    }
}
