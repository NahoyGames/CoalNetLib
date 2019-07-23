using System.Net;

namespace CoalNetLib.Internal
{
    internal class PendingConnection : Connection
    {
        public readonly long ServerSalt;
        
        public PendingConnection(IPEndPoint ep, NetBase net, ushort id) : base(ep, net, id)
        {
            
        }
    }
}