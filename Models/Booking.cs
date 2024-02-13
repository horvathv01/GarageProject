using System.ComponentModel.DataAnnotations.Schema;

namespace GarageProject.Models
{
    public class Booking
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("UserId")]
        public long UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("ParkingSpaceId")]
        public long ParkingSpaceId { get; set; }
        
        public ParkingSpace ParkingSpace { get; set; }

        public DateTime Start {  get; set; }
        public DateTime End { get; set; }

        public Booking(User user, ParkingSpace parkingSpace, DateTime start, DateTime end, long id = 0)
        {
            User = user;
            UserId = user.Id; 
            ParkingSpace = parkingSpace;
            ParkingSpaceId = parkingSpace.Id;
            Start = start;
            End = end;
            Id = id;
        }

        public Booking()
        {

        }

        public override bool Equals( object? obj )
        {
            return obj is Booking booking
                && booking.Id.Equals( Id )
                && booking.User.Id.Equals( UserId )
                && booking.User.Equals( User )
                && booking.ParkingSpaceId.Equals( ParkingSpaceId )
                && booking.ParkingSpace.Equals( ParkingSpace )
                && booking.Start.Equals( Start )
                && booking.End.Equals( End );
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( Id, UserId, User, ParkingSpaceId, ParkingSpace, Start, End );
        }
    }
}
