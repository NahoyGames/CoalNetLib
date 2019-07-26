using System;
using UnityEngine;

namespace CoalNetLib.Unity
{
    public class Server : MonoBehaviour
    {
        [SerializeField] private NetConfig config;

        private CoalNetLib.Server _server;
        
        private void Awake()
        {
            Instance = this;
            
            DontDestroyOnLoad(this);
            Application.runInBackground = true;
            Application.targetFrameRate = config.UpdateRate;

            _server = new CoalNetLib.Server
            {
                MaxConnections = config.MaxConnections,
                MaxOutgoingPacketSize = config.ServerMaxPacketSize,
                MaxIncomingPacketSize = config.ClientMaxPacketSize,
                Timeout = config.Timeout
            };
            foreach (var packet in config.Packets)
            {
                _server.Serializer.RegisterType(packet);
            }

            _server.Start(config.Port);
        }

        private void Update()
        {
            _server.Update();
        }

        private void OnDestroy()
        {
            _server.Stop();
        }

        /// <summary>
        /// The latest running instance of the server
        /// </summary>
        public static Server Instance { get; private set; }
        
        /// <summary>
        /// Accept new incoming connections?
        /// </summary>
        public bool AcceptConnections
        {
            get => _server.AcceptConnections;
            set => _server.AcceptConnections = value;
        }

        /// <summary>
        /// Is the server started?
        /// </summary>
        public bool Started => _server.Started;

        /// <summary>
        /// Send a packet to a given connection
        /// </summary>
        public void Send(Connection receiver, object packet, Channel channel = Channel.Unreliable)
        {
            _server.Send(receiver, packet, channel);
        }

        /// <summary>
        /// Send a packet to all connected clients
        /// </summary>
        public void Send(object packet, Channel channel = Channel.Unreliable)
        {
            _server.Send(packet, channel);
        }

        /// <summary>
        /// Find a connection with its id
        /// </summary>
        public Connection FindConnection(uint id) => _server.GetConnection(id);

        /// <summary>
        /// Subscribe a method be a connection listener
        /// </summary>
        public void Subscribe(Action<Connection> method) => _server.ConnectListeners += method;
        
        /// <summary>
        /// Subscribe a method be a packet listener
        /// </summary>
        public void Subscribe(Action<Connection, object> method) => _server.PacketListeners += method;
        
        /// <summary>
        /// Subscribe a method be a connection listener
        /// </summary>
        public void Subscribe(Action<Connection, DisconnectReason> method) => _server.DisconnectListeners += method;
    }
}