using System;
using System.Net;
using System.Net.Sockets;
using CoalNetLib.Internal;
using FurnaceSerializer;

namespace CoalNetLib
{
    public delegate void ConnectionHandler(Connection connection);
    public delegate void PacketReceivedHandler(Connection sender, object packet);
    public delegate void DisconnectionHandler(Connection connection);

    public abstract class NetBase
    {
        /// <summary>
        /// The socket for this server/client
        /// </summary>
        protected UdpClient Socket { get; private set; }

        /// <summary>
        /// The read/write buffer
        /// </summary>
        protected byte[] Buffer { get; private set; }
        
        /// <summary>
        /// The port of this server/client
        /// </summary>
        public int Port { get; private set; }

        /// <summary>
        /// The serializer used by this server/client
        /// </summary>
        public Serializer Serializer { get; }

        /// <summary>
        /// Methods subscribed to successful & accepted connections
        /// </summary>
        private event ConnectionHandler ConnectionListeners;
        
        /// <summary>
        /// Methods subscribed to incoming packets from successful & accepted connections
        /// </summary>
        private event PacketReceivedHandler PacketListeners;

        /// <summary>
        /// Methods subscribed to disconnections of previously successful & accepted connections
        /// </summary>
        private event DisconnectionHandler DisconnectionListeners;

        /// <summary>
        /// Create a new instance of the coal server
        /// </summary>
        protected NetBase()
        {
            Serializer = new Serializer();
            
            // Register internal packets
            Serializer.RegisterType(typeof(PacketConnectionRequest));
            Serializer.RegisterType(typeof(PacketConnectionRejected));
            Serializer.RegisterType(typeof(PacketConnectionChallenge));
        }

        /// <summary>
        /// Subscribe to incoming successful & accepted connections
        /// </summary>
        public void Subscribe(ConnectionHandler handler) => ConnectionListeners += handler;
        
        /// <summary>
        /// Subscribe to incoming packets
        /// </summary>
        public void Subscribe(PacketReceivedHandler handler) => PacketListeners += handler;

        /// <summary>
        /// Subscribe to disconnection events of previously successful & accepted connections
        /// </summary>
        public void Subscribe(DisconnectionHandler handler) => DisconnectionListeners += handler;

        /// <summary>
        /// Invoke new accepted & successful connection
        /// </summary>
        protected void InvokeConnection(Connection connection) => ConnectionListeners?.Invoke(connection);
        
        /// <summary>
        /// Invoke packet listeners
        /// </summary>
        protected void InvokePacket(Connection sender, object packet) => PacketListeners?.Invoke(sender, packet);

        /// <summary>
        /// Invoke disconnection of previously accepted & successful connection
        /// </summary>
        protected void InvokeDisconnection(Connection connection) => DisconnectionListeners?.Invoke(connection);
        
        /// <summary>
        /// Start listening for incoming data
        /// </summary>
        protected void StartListening(UdpClient socket, int port)
        {
            Port = port;
            Socket = socket;
            Socket.BeginReceive(OnReceiveData, null);
        }

        /// <summary>
        /// Deserialize accordingly the received data and invoke the packet listeners if appropriate
        /// </summary>
        protected abstract void ProcessReceivedData(IPEndPoint sender, byte[] data);

        private void OnReceiveData(IAsyncResult result)
        {
            // Who sent data?
            IPEndPoint remote = new IPEndPoint(IPAddress.Any, Port);
            
            // Data received
            byte[] received = Socket.EndReceive(result, ref remote);
            
            // Begin receiving before processing in case packets are sent while processing occurs
            Socket.BeginReceive(OnReceiveData, null);
            
            // Process received data
            ProcessReceivedData(remote, received);
        }

        /// <summary>
        /// Send data to the given end point
        /// </summary>
        internal void Send(IPEndPoint receiver, byte[] data, int length)
        {
            Socket.Send(data, length, receiver);
        }

        internal void Send(IPEndPoint receiver, object value)
        {
            Serializer.Serialize(value, out int length, 0, Buffer);
            
            Send(receiver, Buffer, length);
        }
    }
}