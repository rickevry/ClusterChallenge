using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace DAM2.Core.Actors.Shared
{
    public class MyIP
    {

        public static string GetMyIp()
        {
            var name = Dns.GetHostName(); // get container id
            IPAddress ip = Dns.GetHostEntry(name).AddressList.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);
            return ip.ToString();
        }
    }
}
