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

            while (!Console.KeyAvailable)
            {
                server.Update();
                
                Thread.Sleep(10);
            }
        }
    }
}