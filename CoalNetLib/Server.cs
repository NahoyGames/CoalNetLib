using System;
using System.Collections.Generic;
using ENet;
using FurnaceSerializer;

namespace CoalNetLib
{
    public class Server
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
        /// Seconds until timeout
        /// </summary>
        public int Timeout { get; set; } = 15;

        /// <summary>
        /// Maximum size in bytes received packets can be
        /// </summary>
        public uint MaxIncomingPacketSize
        {
            get => _maxIncomingPacketSize;
            set
            {
                _maxIncomingPacketSize = value;
                _socket.SetBandwidthLimit(MaxIncomingPacketSize, MaxOutgoingPacketSize);
            }
        }
        private uint _maxIncomingPacketSize = 256;

        /// <summary>
        /// Maximum size in bytes outgoing packets can be
        /// </summary>
        public uint MaxOutgoingPacketSize
        {
            get => _maxOutgoingPacketSize;
            set
            {
                _maxOutgoingPacketSize = value;
                _socket.SetBandwidthLimit(MaxIncomingPacketSize, MaxOutgoingPacketSize);
            }
        }
        private uint _maxOutgoingPacketSize = 512;
        
        /// <summary>
        /// The serializer for this server
        /// </summary>
        public Serializer Serializer { get; }
        
        /// <summary>
        /// ENet socket
        /// </summary>
        private readonly Host _socket;

        /// <summary>
        /// Active connections
        /// </summary>
        private Connection[] _connections;

        /// <summary>
        /// Write buffer
        /// </summary>
        private byte[] _writeBuffer;

        /// <summary>
        /// Read buffer
        /// </summary>
        private byte[] _readBuffer;
        
        /// <summary>
        /// Create a new server instance
        /// </summary>
        public Server()
        {
            Library.Initialize(); // Init ENet
            
            _socket = new Host();
            
            Serializer = new Serializer();
        }

        /// <summary>
        /// Start the server & listen for incoming connections
        /// </summary>
        public void Start(ushort port, bool acceptConnections = true)
        {
            _connections = new Connection[MaxConnections];
            
            _socket.Create
                (
                    new Address { Port = port },
                    MaxConnections,
                    Enum.GetNames(typeof(Channel)).Length,
                    MaxIncomingPacketSize,
                    MaxOutgoingPacketSize
                );
            
            _writeBuffer = new byte[MaxOutgoingPacketSize];
            _readBuffer = new byte[MaxIncomingPacketSize];
            
            AcceptConnections = acceptConnections;
            Started = true;
        }

        /// <summary>
        /// Poll events & notify listeners
        /// </summary>
        public void Update()
        {
            var polled = false;

            while (!polled)
            {
                if (_socket.CheckEvents(out Event @event) <= 0)
                {
                    if (_socket.Service(Timeout, out @event) <= 0) { break; }

                    polled = true;
                }

                switch (@event.Type)
                {
                    case EventType.None:
                        break;
                    case EventType.Connect: InvokeConnect(@event.Peer);
                        break;
                    case EventType.Disconnect: InvokeDisconnect(@event.Peer, DisconnectReason.Default);
                        break;
                    case EventType.Receive: InvokePacket(@event.Peer, @event.Packet);
                        break;
                    case EventType.Timeout: InvokeDisconnect(@event.Peer, DisconnectReason.Timeout);
                        break;
                }
            }
        }

        /// <summary>
        /// Notify connect listeners
        /// </summary>
        private void InvokeConnect(Peer peer)
        {
            if (AcceptConnections)
            {
                _connections[peer.ID] = new Connection(peer);
                ConnectListeners?.Invoke(GetConnection(peer.ID));
            }
            else
            {
                peer.Disconnect((ushort)DisconnectReason.ConnectionClosed);
            }
        }

        /// <summary>
        /// Notify packet listeners
        /// </summary>
        private void InvokePacket(Peer sender, Packet packet)
        {
            packet.CopyTo(_readBuffer);
            PacketListeners?.Invoke(GetConnection(sender), Serializer.Deserialize(_readBuffer));
        }

        /// <summary>
        /// Notify disconnect listeners
        /// </summary>
        private void InvokeDisconnect(Peer peer, DisconnectReason reason)
        {
            DisconnectListeners?.Invoke(GetConnection(peer), reason);
        }

        /// <summary>
        /// Send a packet to a given connection
        /// </summary>
        public void Send(Connection receiver, object packet, Channel channel = Channel.Unreliable)
        {
            var payload = GetPacket(packet, channel);

            receiver._peer.Send((byte) channel, ref payload);
        }

        /// <summary>
        /// Send a packet to everyone, with the option to exclude specific connections
        /// </summary>
        public void Send(object packet, Channel channel = Channel.Unreliable, Connection[] exclude = null)
        {
            var payload = GetPacket(packet, channel);

            if (exclude == null)
            {
                _socket.Broadcast((byte) channel, ref payload);
            }
            else
            {
                var peers = new Peer[exclude.Length];
                for (int i = 0; i < peers.Length; i++)
                {
                    peers[i] = exclude[i]._peer;
                }
                _socket.Broadcast((byte) channel, ref payload, peers);
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
        /// Generate an ENet packet from CoalNetLib packet args
        /// </summary>
        private Packet GetPacket(object packet, Channel channel)
        {
            Serializer.Serialize(packet, out int length, 0, _writeBuffer);
            
            var payload = new Packet();
            payload.Create(_writeBuffer, length, GetFlags(channel));

            return payload;
        }

        /// <summary>
        /// Get the flags for the GetPacket() method
        /// </summary>
        private PacketFlags GetFlags(Channel channel)
        {
            return channel == Channel.Reliable ? PacketFlags.Reliable : PacketFlags.None;
        }

        /// <summary>
        /// Methods listening for connections
        /// </summary>
        public event ConnectHandler ConnectListeners;
        public delegate void ConnectHandler(Connection connection);
        
        /// <summary>
        /// Methods listening for incoming packets
        /// </summary>
        public event ReceivePacketHandler PacketListeners;
        public delegate void ReceivePacketHandler(Connection sender, object packet);

        /// <summary>
        /// Methods listening for disconnections
        /// </summary>
        public event DisconnectHandler DisconnectListeners;
        public delegate void DisconnectHandler(Connection connection, DisconnectReason reason);
        
        /// <summary>
        /// Deconstructor cleanup
        /// </summary>
        ~Server()
        {
            _socket?.Flush();
            _socket?.Dispose();
            
            Library.Deinitialize(); // De-Init UNet
        }
    }
}