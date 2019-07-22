using System.Net;
using System.Net.Sockets;
using CoalNetLib.Internal;

namespace CoalNetLib
{
    public class Server : NetBase
    {
        public Server()
        {
            // Register internal packets
            Serializer.RegisterType(typeof(PacketConnectionRequest));
        }

        /// <summary>
        /// Maximum number of clients that can connect to this server at a time
        /// </summary>
        public int MaxConnections { get; set; } = 10;
        
        /// <summary>
        /// Listen for incoming packets & connections on the given port
        /// </summary>
        /// <param name="port"></param>
        public void Host(int port)
        {
            StartListening(new UdpClient(port), port);
        }

        protected override void ProcessReceivedData(IPEndPoint sender, byte[] data)
        {
            object packet = Serializer.Deserialize(data);
            // TODO connection 
        }
    }
}