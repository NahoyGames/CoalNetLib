using FurnaceSerializer;

namespace CoalNetLib.Sample.Common
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