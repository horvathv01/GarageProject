using PsychAppointments_API.Models.Enums;
using System.Net;
using System.Net.Sockets;

namespace PsychAppointments_API.Service
{
    public static class IPManager
    {
        public static IPAddress GetIpAddress()
        {
            return Dns.GetHostEntry( Dns.GetHostName() )
            .AddressList
            .FirstOrDefault( ip => ip.AddressFamily == AddressFamily.InterNetwork )
            ?? IPAddress.Parse( "127.0.0.1" );
        }

        public static string GenerateURL( URLType? type = URLType.http, IPAddress? address = null, string? port = "5082" )
        {
            if( address == null )
            {
                address = GetIpAddress();
            }
            switch( type )
            {
                case URLType.https:
                    return $"https://{address}:{port}";
                default:
                    return $"http://{address}:{port}";
            }

        }
    }
}
