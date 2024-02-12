using GarageProject.Models;
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

        public BookingDTO( long userId, string start, string end, ParkingSpace? parkingSpace = null, long id = 0 )
        {
            UserId = userId;
            ParkingSpace = parkingSpace;
            Start = start;
            End = end;
            Id = id;
        }

        public BookingDTO( Booking booking )
        {
            Id = booking.Id;
            UserId = booking.UserId;
            ParkingSpace = booking.ParkingSpace;
            Start = booking.Start.ToString( "YYYY\\-MM\\-dd\\HH\\-mm\\-ss" ); //or maybe "yyyy-MM-dd-HH-mm-ss"
            End = booking.End.ToString( "YYYY\\-MM\\-dd\\HH\\-mm\\-ss" );
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
            throw new NotImplementedException();
        }
    }
}
