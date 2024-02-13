using System.ComponentModel.DataAnnotations.Schema;

namespace GarageProject.Models
{
    public class ParkingSpace
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public bool IsDeleted { get; set; } = false;

        public override bool Equals( object? obj )
        {
            return obj is ParkingSpace space &&
                     Id == space.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine( Id );
        }

        public override string ToString()
        {
            return $"Parking Space Id: {Id}, IsDeleted: {IsDeleted}";
        }
    }
}
