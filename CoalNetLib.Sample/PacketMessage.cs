using FurnaceSerializer;

namespace CoalNetLib.Sample
{
    public struct PacketMessage
    {
        [FurnaceSerializable] private string _message;
        public string Message => _message;

        public PacketMessage(string message)
        {
            _message = message;
        }
    }
}