using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;

namespace CoalNetLib.Udp
{
    public class UdpListener : IListener
    {
        private UdpClient _socket;
        private int _port;

        private ConcurrentQueue<IConnection> _connections;

        // Listens for packets but only accepts connection packets
        public void BeginListen(int port)
        {
            _port = port;
            _socket = new UdpClient(_port);

            _socket.BeginReceive(OnReceive, null);
        }

        private void OnReceive(IAsyncResult result)
        {
            var remote = new IPEndPoint(IPAddress.Any, _port);
            
            var received = _socket.EndReceive(result, ref remote);
            _socket.BeginReceive(OnReceive, null);
            
            // Process received data
            Console.WriteLine("Received data from {0} of length {1}", remote.Address, received.Length);
        }

        public IConnection GetNextConnection()
        {
            return _connections.TryPeek(out var result) ? result : null;
        }
    }
}