using ENet;

namespace CoalNetLib
{
    public class Connection
    {
        public uint Id => Peer.ID;
        
        internal Peer Peer;

        public Connection(Peer peer)
        {
            Peer = peer;
        }
    }
}