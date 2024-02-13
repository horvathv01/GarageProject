
using GarageProject.Models;

namespace GarageProject.Converters
{
    public class DateTimeConverter : IDateTimeConverter
    {
        public DateTime Convert( string date )
        {
            var today = DateTime.Today;

            switch ( date )
            {
                case "today":
                    return today;
                case "now":
                    return DateTime.Now;
                case "endOfToday":
                    return new DateTime( today.Year, today.Month, today.Day, 23, 59, 0 );
                case "tomorrow":
                    return today.AddDays( 1 );
                default:
                    DateTime dateParsed;
                    if (!DateTime.TryParse( date, out dateParsed ))
                    {
                        throw new Exception( "Date parsing failed" );
                    }
                    dateParsed = DateTime.SpecifyKind( dateParsed, DateTimeKind.Utc );
                    return dateParsed;
            }
        }

        public string Convert( DateTime date, string formatString = "yyyy-MM-dd-HH-mm-ss" )
        {
            return date.ToString( formatString );
        }
    }
}
