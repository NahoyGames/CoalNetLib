namespace CoalNetLib
{
    public enum DisconnectReason : ushort
    {
        Default,
        Timeout,
        Kicked,
        ConnectionClosed,
        ServerClosed
    }
}