using GarageProject.Models;
using GarageProject.Models.DTOs;

namespace GarageProject.Converters
{
    public class BookingConverter : IBookingConverter
    {
        public BookingDTO? ConvertToBookingDTO( Booking? booking )
        {
            return booking == null ? null : new BookingDTO( booking );
        }

        public IEnumerable<BookingDTO>? ConvertToBookingDTOIEnumerable( IEnumerable<Booking>? bookings )
        {
            return bookings?.Select(b => new BookingDTO( b ) ).ToList() ?? new List<BookingDTO>();
        }
    }
}
