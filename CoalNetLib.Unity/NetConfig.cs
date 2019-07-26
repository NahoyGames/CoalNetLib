using System;
using UnityEngine;

namespace CoalNetLib.Unity
{
    [CreateAssetMenu(fileName = "New Network Configuration", menuName = "CoalNetLib/NetConfig")]
    public class NetConfig : ScriptableObject
    {
        /// <summary>
        /// The port the server is running on
        /// </summary>
        public ushort Port => port;
        [SerializeField] private ushort port;

        /// <summary>
        /// Time to timeout
        /// </summary>
        public int Timeout => timeout;
        [SerializeField] private int timeout;

        /// <summary>
        /// Array of registered packets
        /// </summary>
        public Type[] Packets => packets;
        [SerializeField] private Type[] packets;
        
        /// <summary>
        /// Maximum number of connections on the server at a time
        /// </summary>
        public int MaxConnections => maxConnections;
        [SerializeField] private int maxConnections;

        /// <summary>
        /// Refresh rate of the server/client(fps)
        /// </summary>
        public int UpdateRate => updateRate;
        [SerializeField] private int updateRate;
        
        /// <summary>
        /// Maximum size of packets outgoing from the server
        /// </summary>
        public uint ServerMaxPacketSize => serverMaxPacketSize;
        [SerializeField] private uint serverMaxPacketSize;
        
        /// <summary>
        /// Maximum size of packets outgoing from the client
        /// </summary>
        public uint ClientMaxPacketSize => clientMaxPacketSize;
        [SerializeField] private uint clientMaxPacketSize;
    }
}