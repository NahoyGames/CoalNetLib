using System;
using ENet;

namespace CoalNetLib
{
    public class Server : NetBase
    {
        /// <summary>
        /// Maximum number of connections at a time
        /// </summary>
        public int MaxConnections
        {
            get => _maxConnections;
            set
            {
                if (Started)
                {
                    throw new Exception("Cannot modify the max number of connections while the server is started!");
                }

                _maxConnections = value;
            }
        }
        private int _maxConnections = 10;
        
        /// <summary>
        /// Accept incoming connections?
        /// </summary>
        public bool AcceptConnections { get; set; } = false;

        /// <summary>
        /// Is the server started?
        /// </summary>
        public bool Started { get; private set; } = false;
        
        /// <summary>
        /// Active connections
        /// </summary>
        private Connection[] _connections;
        
        /// <summary>
        /// Start the server & listen for incoming connections
        /// </summary>
        public void Start(ushort port, bool acceptConnections = true)
        {
            _connections = new Connection[MaxConnections];
            
            Socket.Create
                (
                    new Address { Port = port },
                    MaxConnections,
                    Enum.GetNames(typeof(Channel)).Length,
                    MaxIncomingPacketSize,
                    MaxOutgoingPacketSize
                );
            
            WriteBuffer = new byte[MaxOutgoingPacketSize];
            ReadBuffer = new byte[MaxIncomingPacketSize];
            
            AcceptConnections = acceptConnections;
            Started = true;
        }

        /// <summary>
        /// Stop the server
        /// </summary>
        public void Stop()
        {
            foreach (var connection in _connections)
            {
                Disconnect(connection, DisconnectReason.ServerClosed);
            }
            Socket.Dispose();
        }

        /// <summary>
        /// Disconnect a client and remove its connection
        /// </summary>
        public void Disconnect(Connection connection, DisconnectReason reason)
        {
            if (connection != null)
            {
                connection.Peer.Disconnect(0);
                _connections[connection.Id] = null;
            }
        }
        
        protected override void InvokeConnect(Peer peer)
        {
            if (AcceptConnections)
            {
                _connections[peer.ID] = new Connection(peer);
                ConnectListeners?.Invoke(GetConnection(peer.ID));
            }
            else
            {
                peer.Disconnect(0); // (ushort)DisconnectReason.ConnectionClosed
            }
        }
        
        protected override void InvokePacket(Peer sender, Packet packet)
        {
            packet.CopyTo(ReadBuffer);
            PacketListeners?.Invoke(GetConnection(sender), Serializer.Deserialize(ReadBuffer));
            packet.Dispose();
        }
        
        protected override void InvokeDisconnect(Peer peer, DisconnectReason reason)
        {
            DisconnectListeners?.Invoke(GetConnection(peer), reason);
        }

        /// <summary>
        /// Send a packet to a given connection
        /// </summary>
        public void Send(Connection receiver, object packet, Channel channel = Channel.Unreliable)
        {
            var payload = GetPacket(packet, channel);

            receiver.Peer.Send((byte) channel, ref payload);
        }

        /// <summary>
        /// Send a packet to everyone, with the option to exclude specific connections
        /// </summary>
        public void Send(object packet, Channel channel = Channel.Unreliable, Connection[] exclude = null)
        {
            var payload = GetPacket(packet, channel);

            if (exclude == null)
            {
                Socket.Broadcast((byte) channel, ref payload);
            }
            else
            {
                var peers = new Peer[exclude.Length];
                for (int i = 0; i < peers.Length; i++)
                {
                    peers[i] = exclude[i].Peer;
                }
                Socket.Broadcast((byte) channel, ref payload, peers);
            }
        }

        /// <summary>
        /// Get a connection by its server-issued id
        /// </summary>
        public Connection GetConnection(uint id) => _connections[id];

        /// <summary>
        /// Get a connection by its ENet peer component
        /// </summary>
        internal Connection GetConnection(Peer peer) => GetConnection(peer.ID);

        /// <summary>
        /// Methods listening for connections
        /// </summary>
        public event Action<Connection> ConnectListeners;
        
        /// <summary>
        /// Methods listening for incoming packets
        /// </summary>
        public event Action<Connection, object> PacketListeners;

        /// <summary>
        /// Methods listening for disconnections
        /// </summary>
        public event Action<Connection, DisconnectReason> DisconnectListeners;
    }
}