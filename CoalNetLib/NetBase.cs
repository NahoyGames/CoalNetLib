using ENet;
using FurnaceSerializer;

namespace CoalNetLib
{
    public abstract class NetBase
    {
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
                Socket.SetBandwidthLimit(MaxIncomingPacketSize, MaxOutgoingPacketSize);
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
                Socket.SetBandwidthLimit(MaxIncomingPacketSize, MaxOutgoingPacketSize);
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
        protected readonly Host Socket;
        
        /// <summary>
        /// Write buffer
        /// </summary>
        protected byte[] WriteBuffer;

        /// <summary>
        /// Read buffer
        /// </summary>
        protected byte[] ReadBuffer;

        /// <summary>
        /// Create a new netbase instance
        /// </summary>
        protected NetBase()
        {
            Library.Initialize(); // Init ENet
            
            Socket = new Host();
            
            Serializer = new Serializer();
        }

        /// <summary>
        /// Notify connect listeners
        /// </summary>
        protected abstract void InvokeConnect(Peer peer);

        /// <summary>
        /// Notify packet listeners
        /// </summary>
        protected abstract void InvokePacket(Peer sender, Packet packet);

        /// <summary>
        /// Notify disconnect listeners
        /// </summary>
        protected abstract void InvokeDisconnect(Peer peer, DisconnectReason reason);
        
        /// <summary>
        /// Poll events & notify listeners
        /// </summary>
        public void Update()
        {
            var polled = false;

            while (!polled)
            {
                if (Socket.CheckEvents(out Event @event) <= 0)
                {
                    if (Socket.Service(Timeout, out @event) <= 0) { break; }

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
        /// Generate an ENet packet from CoalNetLib packet args
        /// </summary>
        protected Packet GetPacket(object packet, Channel channel)
        {
            Serializer.Serialize(packet, out int length, 0, WriteBuffer);
            
            var payload = new Packet();
            payload.Create(WriteBuffer, length, GetFlags(channel));

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
        /// Deconstructor cleanup
        /// </summary>
        ~NetBase()
        {
            Socket?.Flush();
            Socket?.Dispose();
            
            Library.Deinitialize(); // De-Init UNet
        }
    }
}