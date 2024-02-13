using GarageProject.Models;
using GarageProject.Models.DTOs;

namespace GarageProject.Converters
{
    public class BookingConverter : IBookingConverter
    {
        private readonly IDateTimeConverter _dateTimeConverter;

        public BookingConverter(IDateTimeConverter dateTimeConverter)
        {
            _dateTimeConverter = dateTimeConverter;
        }

        public BookingDTO? ConvertToBookingDTO( Booking? booking )
        {
            return booking == null ? null : new BookingDTO( booking );
        }

        public IEnumerable<BookingDTO>? ConvertToBookingDTOIEnumerable( IEnumerable<Booking>? bookings )
        {
            return bookings?.Select(b => new BookingDTO()
            {
                Id = b.Id,
                UserId = b.UserId,
                ParkingSpace = b.ParkingSpace,
                Start = _dateTimeConverter.Convert( b.Start ),
                End = _dateTimeConverter.Convert( b.End )
            }).ToList() ?? new List<BookingDTO>();
        }
    }
}
