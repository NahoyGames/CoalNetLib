using System.Net.Sockets;

namespace CoalNetLib
{
    public class CoalServer
    {
        private UdpClient _socket;
        private FurnaceSerializer _serializer;
        
        /// <summary>
        /// Create a new instance of the coal server
        /// </summary>
        public CoalServer()
        {
            
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
            _socket = new UdpClient(port);
        }
    }
}