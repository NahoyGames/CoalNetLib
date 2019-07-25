using System;
using ENet;

namespace CoalNetLib
{
    public class Client : NetBase
    {
        /// <summary>
        /// Is the client connected to the server?
        /// </summary>
        public bool Connected => _server?.Peer.State == PeerState.Connected;
        
        /// <summary>
        /// Is the client connecting to the server?
        /// </summary>
        public bool Connecting => _server?.Peer.State == PeerState.Connecting;

        /// <summary>
        /// Connection to server
        /// </summary>
        private Connection _server;

        /// <summary>
        /// Connect to the server
        /// </summary>
        public void Connect(string ip, ushort port)
        {
            var address = new Address { Port = port };
            address.SetHost(ip);
            
            Socket.Create();

            _server = new Connection(Socket.Connect(address, Enum.GetNames(typeof(Channel)).Length));
            
            WriteBuffer = new byte[MaxOutgoingPacketSize];
            ReadBuffer = new byte[MaxIncomingPacketSize];
        }

        /// <summary>
        /// Disconnect from the server
        /// </summary>
        public void Disconnect()
        {
            if (_server != null)
            {
                _server.Peer.Disconnect(0);
                _server = null;
            }
        }

        protected override void InvokeConnect(Peer peer)
        {
            if (peer.IP == _server.Peer.IP)
            {
                ConnectListeners?.Invoke();
            }
            else
            {
                peer.Disconnect(0);
            }
        }

        protected override void InvokePacket(Peer sender, Packet packet)
        {
            packet.CopyTo(ReadBuffer);
            PacketListeners?.Invoke(Serializer.Deserialize(ReadBuffer));
            packet.Dispose();
        }

        protected override void InvokeDisconnect(Peer peer, DisconnectReason reason)
        {
            DisconnectListeners?.Invoke(reason);
        }

        /// <summary>
        /// Send packets to the server
        /// </summary>
        public void Send(object packet, Channel channel = Channel.Unreliable)
        {
            var payload = GetPacket(packet, channel);

            _server.Peer.Send((byte) channel, ref payload);
        }
        
        /// <summary>
        /// Methods listening for connections
        /// </summary>
        public event Action ConnectListeners;
        
        /// <summary>
        /// Methods listening for incoming packets
        /// </summary>
        public event Action<object> PacketListeners;

        /// <summary>
        /// Methods listening for disconnections
        /// </summary>
        public event Action<DisconnectReason> DisconnectListeners;
    }
}