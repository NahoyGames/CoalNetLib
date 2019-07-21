using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CoalNetLib.Udp;

namespace CoalNetLib.Sample
{
    class Program
    {
        private static void Main(string[] args)
        {
            var server = new CoalServer(new UdpListener());
            
            server.Start(8000);

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey().Key;

                    if (key == ConsoleKey.A)
                    {
                        using (var socket = new UdpClient())
                        {
                            socket.Connect(new IPEndPoint(IPAddress.Parse("192.168.1.83"), 8000));
                            socket.Send(new byte[8], 8);
                        }
                    }
                    else if (key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }
                
                Thread.Sleep(10);
            }
        }
    }
}