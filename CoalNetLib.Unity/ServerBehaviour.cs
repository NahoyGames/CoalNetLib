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
        }
        
        protected virtual void OnConnection(Connection connection) { }
        
        protected virtual void OnPacket(Connection sender, object packet) { }
        
        protected virtual void OnDisconnection(Connection connection, DisconnectReason reason) { }
    }
}