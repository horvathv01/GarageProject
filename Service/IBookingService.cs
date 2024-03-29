﻿using GarageProject.Models;
using GarageProject.Models.DTOs;

namespace GarageProject.Service
{
    public interface IBookingService
    {
        Task<bool> AddBooking( BookingDTO booking );
        Task<bool> AddBooking( User user, DateTime startDate, DateTime endDate, ParkingSpace? parkingSpace = null );
        Task<Booking?> GetBookingById( long id );
        Task<IEnumerable<Booking>?> GetBookingsByDates( string startDate, string endDate );
        Task<IEnumerable<Booking>?> GetBookingsByDates( DateTime startDate, DateTime endDate );
        Task<IEnumerable<Booking>?> GetAllBookings();
        Task<IEnumerable<Booking>?> GetBookingsByUser( long userId );
        Task<IEnumerable<Booking>?> GetBookingsByUser( long userId, string startDate, string endDate );
        Task<IEnumerable<Booking>?> GetBookingsByUser( User user, DateTime startDate, DateTime endDate );
        Task<IEnumerable<Booking>?> GetListOfBookings( List<long> ids );
        Task<IEnumerable<ParkingSpace>?> GetAvailableParkingSpacesForDate( DateTime date );
        Task<IEnumerable<ParkingSpace>?> GetAvailableParkingSpacesForDate( string date );
        Task<IEnumerable<ParkingSpace>?> GetAvailableParkingSpacesForTimeRange( DateTime startDate, DateTime endDate );
        Task<IEnumerable<ParkingSpace>?> GetAvailableParkingSpacesForTimeRange( string startDate, string endDate );
        Task<int> GetNumberOfEmptySpacesForDate( string date );
        Task<int> GetNumberOfEmptySpacesForDate( DateTime date );
        Task<IEnumerable<DateTime>> GetFullDaysOfMonth( string? date = null );
        Task<bool> IsParkingSpaceFree( ParkingSpace space, DateTime start, DateTime end, long? bookingId = null );
        Task<bool> RemoveDayFromBooking( long bookingId, string date, long userId );
        Task<bool> FillDaysWithBookings( long loggedInUserId, long userId, string startDateString, string endDateString, ParkingSpace? parkingSpace = null );
        Task<bool> RemoveBookingsFromDaysInRange( long loggedInUserId, long userId, string startDateString, string endDateString );
        Task<bool> UpdateBooking( long id, BookingDTO newBooking, long userId );
        Task<bool> DeleteBooking( long id, long userId );
    }
}