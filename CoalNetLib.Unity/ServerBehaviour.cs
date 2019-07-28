using System;
using FurnaceSerializer;
using UnityEngine;

namespace CoalNetLib.Unity
{
    public class ServerBehaviour : MonoBehaviour
    {
        private void Awake()
        {
            if (Server.Instance == null)
            {
                Debug.LogError("Cannot awake a ServerBehavior before the Server. Check your script execution order.");
                return;
            }
            Server.Instance.Subscribe(OnConnection);
            Server.Instance.Subscribe((Action<Connection, object>) OnPacket);
            Server.Instance.Subscribe(OnDisconnection);

            Identity = GetComponent<NetIdentity>();
        }
        
        public NetIdentity Identity { get; private set; }

        protected void SendPacket(Connection receiver, object packet, Channel channel = Channel.Unreliable)
        {
            Server.Instance.Send(receiver, packet, channel);
        }
        
        protected virtual void OnConnection(Connection connection) { }
        
        protected virtual void OnPacket(Connection sender, object packet) { }
        
        protected virtual void OnDisconnection(Connection connection, DisconnectReason reason) { }
    }
}