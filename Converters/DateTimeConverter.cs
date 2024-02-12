
using GarageProject.Models;

namespace GarageProject.Converters
{
    public class DateTimeConverter : IDateTimeConverter
    {
        public DateTime Convert( string date )
        {
            if(date == "today")
            {
                return DateTime.Today;
            }
            if(date == "now")
            {
                return DateTime.Now;
            }

            DateTime dateParsed;
            if (!DateTime.TryParse( date, out dateParsed ))
            {
                throw new Exception( "Date parsing failed" );
            }
            dateParsed = DateTime.SpecifyKind( dateParsed, DateTimeKind.Utc );
            return dateParsed;
        }
    }
}
