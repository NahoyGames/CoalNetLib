using System;
using System.Net;

namespace CoalNetLib
{
    public class Connection : IEquatable<Connection>
    {
        private readonly NetBase _net;

        /// <summary>
        /// The server issued identifier of this connection
        /// </summary>
        public readonly int Id;
        
        /// <summary>
        /// The IP address of this connection
        /// </summary>
        public IPAddress Address { get; }
        
        /// <summary>
        /// The IP end point of this connection
        /// </summary>
        internal readonly IPEndPoint EndPoint;
        
        /// <summary>
        /// Create a new connection
        /// </summary>
        internal Connection(IPEndPoint ep, NetBase net, int id)
        {
            _net = net;
            EndPoint = ep;
            Id = id;
        }

        /// <summary>
        /// Send to this connection
        /// </summary>
        public void Send(byte[] data, int length)
        {
            _net.Send(EndPoint, data, length);
        }

        public bool Equals(Connection other)
        {
            return Address.Equals(other.Address);
        }
    }
}