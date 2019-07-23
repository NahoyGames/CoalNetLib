using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using CoalNetLib.Internal;

namespace CoalNetLib
{
    public class Server : NetBase
    {
        private readonly ConcurrentDictionary<IPEndPoint, Connection> _connections;
        private readonly ConcurrentDictionary<IPEndPoint, PendingConnection> _pendingConnections;
        
        public Server()
        {
            // Connections
            _connections = new ConcurrentDictionary<IPEndPoint, Connection>();
            _pendingConnections = new ConcurrentDictionary<IPEndPoint, PendingConnection>();
        }

        /// <summary>
        /// Maximum number of clients that can connect to this server at a time
        /// </summary>
        public int MaxConnections { get; set; } = 10;

        /// <summary>
        /// Maximum number of clients that can attempt to connect to this server at a time
        /// </summary>
        public int MaxPendingConnections { get; set; } = 5;
        
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

            if (!_connections.ContainsKey(sender)) // New connection
            {
                ProcessIncomingConnection(sender, packet);
            }
        }

        private void ProcessIncomingConnection(IPEndPoint sender, object packet)
        {
            // Auto reject
            if (_connections.Count >= MaxConnections)
            {
                Send(sender, new PacketConnectionRejected(PacketConnectionRejected.Reasons.ServerFull));
            }
            else if (_pendingConnections.Count >= MaxPendingConnections)
            {
                Send(sender, new PacketConnectionRejected(PacketConnectionRejected.Reasons.PendingLimitReached));
            }
            
            // Challenge
            
        }
    }
}