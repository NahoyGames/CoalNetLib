using System;
using System.Threading;
using CoalNetLib.Sample.Common;

namespace CoalNetLib.Sample.Server
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Started @ " + DateTime.Now);
            
            var server = new CoalNetLib.Server();
            
            server.Start(5000);
            server.Serializer.RegisterType(typeof(PacketMessage));

            server.ConnectListeners += connection => Console.WriteLine("Got a connection! " + connection.Id);
            
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey().Key;

                    switch (key)
                    {
                        case ConsoleKey.A:
                            //server.GetConnection()
                            server.Send(server.GetConnection(0), new PacketMessage("Hello clients!"));
                            break;
                        default:
                            server.Stop();
                            return;
                    }
                }
                
                server.Update();
                
                Thread.Sleep(10);
            }
        }
    }
}