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
        public int MaxConnections { get; set; } = 10;

        /// <summary>
        /// Accept incoming connections?
        /// </summary>
        public bool AcceptConnections { get; set; } = false;

        /// <summary>
        /// Seconds until timeout
        /// </summary>
        public int Timeout { get; set; } = 15;

        /// <summary>
        /// Maximum size in bytes received packets can be
        /// </summary>
        public uint MaxPacketSize
        {
            get => _maxPacketSize;
            set
            {
                _maxPacketSize = value;
                _socket.SetBandwidthLimit(MaxPacketSize, BufferSize);
            }
        }
        private uint _maxPacketSize;

        /// <summary>
        /// Size in bytes of the write buffer
        /// </summary>
        public uint BufferSize
        {
            get => _bufferSize;
            set
            {
                _bufferSize = value;
                _socket.SetBandwidthLimit(MaxPacketSize, BufferSize);
            }
        }
        private uint _bufferSize = 512;
        
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
        private readonly IList<Connection> _connections;

        /// <summary>
        /// Write buffer
        /// </summary>
        private byte[] _buffer;
        
        /// <summary>
        /// Create a new server instance
        /// </summary>
        public Server()
        {
            Library.Initialize(); // Init ENet
            
            _socket = new Host();
            _connections = new List<Connection>();
            
            Serializer = new Serializer();
        }

        /// <summary>
        /// Start the server & listen for incoming connections
        /// </summary>
        public void Start(ushort port, bool acceptConnections = true)
        {
            _socket.Create
                (
                    new Address { Port = port },
                    MaxConnections,
                    Enum.GetNames(typeof(Channel)).Length,
                    MaxPacketSize,
                    BufferSize
                );
            _buffer = new byte[BufferSize];
            
            AcceptConnections = acceptConnections;
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
                    case EventType.Disconnect: DisconnectListeners?.Invoke(@event.Peer.ID, DisconnectReason.Default);
                        break;
                    case EventType.Receive: PacketListeners?.Invoke(@event.Peer.ID, @event.Packet);
                        break;
                    case EventType.Timeout: DisconnectListeners?.Invoke(@event.Peer.ID, DisconnectReason.Timeout);
                        break;
                }
            }
        }

        private void InvokeConnect(Peer peer)
        {
            if (AcceptConnections)
            {
                ConnectListeners?.Invoke(peer.ID);
            }
            else
            {
                peer.Disconnect((ushort)DisconnectReason.ConnectionClosed);
            }
        }

        /// <summary>
        /// Send a packet to a given connection
        /// </summary>
        public void Send(Connection receiver, object packet, Channel channel = Channel.Unreliable)
        {
            Serializer.Serialize(packet, out int length, 0, _buffer);
            
            var payload = new Packet();
            payload.Create(_buffer, length, GetFlags(channel));

            receiver._peer.Send((byte) channel, ref payload);
        }

        private PacketFlags GetFlags(Channel channel)
        {
            return channel == Channel.Reliable ? PacketFlags.Reliable : PacketFlags.None;
        }

        /// <summary>
        /// Methods listening for connections
        /// </summary>
        public event ConnectHandler ConnectListeners;
        public delegate void ConnectHandler(uint connection);
        
        /// <summary>
        /// Methods listening for incoming packets
        /// </summary>
        public event ReceivePacketHandler PacketListeners;
        public delegate void ReceivePacketHandler(uint sender, object packet);

        /// <summary>
        /// Methods listening for disconnections
        /// </summary>
        public event DisconnectHandler DisconnectListeners;
        public delegate void DisconnectHandler(uint connection, DisconnectReason reason);
        
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