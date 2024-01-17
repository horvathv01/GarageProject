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
            return obj is ParkingSpace space 
                && space.Id == Id;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"Parking Space Id: {Id}, IsDeleted: {IsDeleted}";
        }
    }
}
