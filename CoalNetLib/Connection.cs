using ENet;

namespace CoalNetLib
{
    public class Connection
    {
        internal Peer _peer;

        public Connection(Peer peer)
        {
            _peer = peer;
        }
    }
}