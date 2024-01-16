
using PsychAppointments_API.Models;

namespace PsychAppointments_API.Converters
{
    public class DateTimeConverter : IDateTimeConverter
    {
        public DateTime Convert( string date )
        {
            DateTime dateParsed;
            if (!DateTime.TryParse( date, out dateParsed ))
            {
                throw new Exception( "Date parsing failed" );
            }
            return dateParsed;
        }
    }
}
