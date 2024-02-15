namespace GarageProject.Converters
{
    public interface IDateTimeConverter
    {
        DateTime Convert( string date );
        string Convert( DateTime date, string? formatString = "yyyy-MM-dd-HH-mm-ss" );
    }
}
