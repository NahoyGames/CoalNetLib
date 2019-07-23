using FurnaceSerializer;

namespace CoalNetLib.Internal
{
    // client -> server
    internal struct PacketConnectionRequest
    {
        [FurnaceSerializable] private long clientSalt;
    }
}