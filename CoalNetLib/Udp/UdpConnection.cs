using System;
using System.Net;
using System.Net.Sockets;

namespace CoalNetLib.Udp
{
    public class UdpConnection : IConnection
    {
        private readonly UdpClient _socket;
        private readonly IPEndPoint _endPoint;
        
        public IPAddress Address { get; }
        
        public bool Connected { get; }
        
        public void Send(byte[] data, int length, params object[] args)
        {
            throw new NotImplementedException();
        }

        public void Listen(Action<byte[]> received)
        {
            throw new NotImplementedException();
        }
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}