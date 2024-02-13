namespace GarageProject.Converters
{
    public interface IDateTimeConverter
    {
        DateTime Convert( string date );
        string Convert( DateTime date );
    }
}
