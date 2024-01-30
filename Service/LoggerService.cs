namespace GarageProject.Service
{
    public class LoggerService : ILoggerService
    {
        public void Log( string message )
        {
            Console.WriteLine( message );
        }
    }
}
