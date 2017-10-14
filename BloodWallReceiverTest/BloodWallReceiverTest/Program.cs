using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace BloodWallReceiverTest
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpClient client = new UdpClient(11000);
            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 11000);

            Console.WriteLine("Listening this will never quit so you will need to ctrl-c to quit!");
            while (true)
            {
                Byte[] data = client.Receive(ref localEp);
                string strData = Encoding.Unicode.GetString(data);
                Console.WriteLine(strData);
            }
        }
    }
}
