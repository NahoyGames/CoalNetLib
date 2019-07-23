using FurnaceSerializer;

namespace CoalNetLib.Internal
{
    // server -> client
    public struct PacketConnectionRejected
    {
        public enum Reasons : byte
        {
            NotSpecified,
            ServerFull,
            PendingLimitReached,
            InvalidChallengeResponse
        }
        public Reasons Reason => (Reasons)reason;

        [FurnaceSerializable] private byte reason;

        public PacketConnectionRejected(Reasons reason)
        {
            this.reason = (byte)reason;
        }
    }
}