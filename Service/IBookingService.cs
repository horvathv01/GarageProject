using GarageProject.Models;
using GarageProject.Models.DTOs;
using PsychAppointments_API.Models;

namespace PsychAppointments_API.Service
{
    public interface IBookingService
    {
        Task<bool> AddBooking( BookingDTO booking );
        Task<Booking?> GetBookingById( long id );
        Task<IEnumerable<Booking>?> GetBookingsByDates( string startDate, string endDate );
        Task<IEnumerable<Booking>?> GetAllBookings();
        Task<IEnumerable<Booking>?> GetBookingsByUser( User user );
        Task<IEnumerable<Booking>?> GetBookingsByUser( User user, string startDate, string endDate );
        Task<IEnumerable<Booking>?> GetListOfBookings( List<long> ids );
        Task<bool> UpdateBooking( Booking booking, BookingDTO newBooking );
        Task<bool> DeleteBooking( Booking booking );
    }
}
