using GarageProject.Models;
using GarageProject.Models.DTOs;

namespace GarageProject.Converters
{
    public interface IBookingConverter
    {
        BookingDTO? ConvertToBookingDTO( Booking? booking );
        IEnumerable<BookingDTO>? ConvertToBookingDTOIEnumerable( IEnumerable<Booking>? bookings );
    }
}
