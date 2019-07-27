using System;
using CoalNetLib.Unity.Serializers;
using UnityEngine;

namespace CoalNetLib.Unity
{
    public class Client : MonoBehaviour
    {
        [SerializeField] private NetConfig config;

        [SerializeField] private string serverIp = "127.0.0.1";
        [SerializeField] private bool connectOnAwake = true;
        
        private CoalNetLib.Client _client;

        private void Awake()
        {
            Instance = this;
            
            DontDestroyOnLoad(this);
            Application.runInBackground = true;
            Application.targetFrameRate = config.UpdateRate;

            _client = new CoalNetLib.Client();

            _client.Timeout = config.Timeout;
            _client.MaxIncomingPacketSize = config.ServerMaxPacketSize;
            _client.MaxOutgoingPacketSize = config.ClientMaxPacketSize;
            
            _client.Serializer.RegisterSerializer(new QuaternionSerializer());
            _client.Serializer.RegisterSerializer(new Vector2Serializer());
            _client.Serializer.RegisterSerializer(new Vector3Serializer());
            _client.Serializer.RegisterSerializer(new Vector4Serializer());
            
            foreach (var packet in config.Packets)
            {
                _client.Serializer.RegisterType(packet);
            }

            if (connectOnAwake)
            {
                Connect();
            }
        }

        private void Update()
        {
            _client.Update();
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        /// <summary>
        /// The latest running instance of the server
        /// </summary>
        public static Client Instance { get; private set; }

        /// <summary>
        /// Connect to the ip & port provided in the config object
        /// </summary>
        public void Connect() => Connect(serverIp);

        /// <summary>
        /// Connect to a runtime-provided ip
        /// </summary>
        public void Connect(string ip) => Connect(ip, config.Port);
        
        /// <summary>
        /// Connect to a runtime-provided ip & port
        /// </summary>
        public void Connect(string ip, ushort port) => _client.Connect(ip, port);
        
        /// <summary>
        /// Disconnect from the server
        /// </summary>
        public void Disconnect() => _client.Disconnect();

        /// <summary>
        /// Is the client connecting?
        /// </summary>
        public bool Connecting => _client.Connecting;

        /// <summary>
        /// Is the client connected?
        /// </summary>
        public bool Connected => _client.Connected;

        /// <summary>
        /// Send a packet to the server
        /// </summary>
        public void Send(object packet, Channel channel = Channel.Unreliable) => _client.Send(packet, channel);
        
        /// <summary>
        /// Subscribe a method be a connection listener
        /// </summary>
        public void Subscribe(Action method) => _client.ConnectListeners += method;
        
        /// <summary>
        /// Subscribe a method be a packet listener
        /// </summary>
        public void Subscribe(Action<object> method) => _client.PacketListeners += method;
        
        /// <summary>
        /// Subscribe a method be a connection listener
        /// </summary>
        public void Subscribe(Action<DisconnectReason> method) => _client.DisconnectListeners += method;
    }
}