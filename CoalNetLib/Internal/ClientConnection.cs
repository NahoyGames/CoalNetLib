using System;
using System.Net;

namespace CoalNetLib.Internal
{
    internal class ClientConnection : Connection
    {
        /// <summary>
        /// Last time a packet was received in milliseconds
        /// </summary>
        public long LastPacketTime { get; private set; }
        
        internal ClientConnection(IPEndPoint ep, NetBase net, ushort id) : base(ep, net, id)
        {
            
        }
    }
}