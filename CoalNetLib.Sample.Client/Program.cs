using System;
using System.Threading;
using CoalNetLib.Sample.Common;

namespace CoalNetLib.Sample.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Started @ " + DateTime.Now);

            var client = new CoalNetLib.Client();
            
            client.Connect("127.0.0.1", 5000);
            client.Serializer.RegisterType(typeof(PacketMessage));

            client.PacketListeners += o =>
                {
                    if (o is PacketMessage message)
                    {
                        Console.WriteLine("Server: " + message.Message);
                    }
                };
            
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    client.Disconnect();
                    break;
                }
                
                client.Update();
                
                Thread.Sleep(10);
            }
        }
    }
}