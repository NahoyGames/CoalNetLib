using System;
using System.Threading;

namespace CoalNetLib.Sample
{
    public class ServerProgram
    {
        public static void Main(string[] args)
        {
            var server = new Server();
            
            server.Start(5000);
            server.Serializer.RegisterType(typeof(PacketMessage));

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey().Key;

                    switch (key)
                    {
                        case ConsoleKey.A:
                            server.Send(new PacketMessage("Hello clients!"));
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