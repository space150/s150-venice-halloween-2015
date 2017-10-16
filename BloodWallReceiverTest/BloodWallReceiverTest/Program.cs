using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace BloodWallReceiverTest
{
    class Program
    {
        static void Main(string[] args)
        {
            //receive();
            send();
        }

        private static void receive()
        {
            UdpClient client = new UdpClient(11000);
            IPEndPoint localEp = new IPEndPoint(IPAddress.Any, 11000);
            
            Console.WriteLine("Listening will never stop so you will need to ctrl-c to quit!");
            while (true)
            {
                Byte[] data = client.Receive(ref localEp);
                string strData = Encoding.ASCII.GetString(data);
                Console.WriteLine(strData);
            }
        }

        private static void send()
        {
            int i = 0;

            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 11000);

            Console.WriteLine("Sending will never stop so you will need to ctrl-c to quit!");
            while (true)
            {
                
                string dataString = string.Format("{0}", i);
                byte[] bytes = Encoding.ASCII.GetBytes(dataString);
                client.Send(bytes, bytes.Length, ip);
                

                Console.WriteLine(dataString);

                i += 1;

                System.Threading.Thread.Sleep(1000);
            }

            client.Close();
        }

    }
}
