using System;
using UnityEngine;

namespace CoalNetLib.Unity
{
    public class ClientBehaviour : MonoBehaviour
    {
        private void Awake()
        {
            if (Client.Instance == null)
            {
                Debug.LogError("Cannot awake a ServerBehavior before the Server. Check your script execution order.");
                return;
            }
            Client.Instance.Subscribe(OnConnected);
            Client.Instance.Subscribe((Action<object>) OnPacket);
            Client.Instance.Subscribe(OnDisconnected);
        }
        
        protected virtual void OnConnected() { }
        
        protected virtual void OnPacket(object packet) { }
        
        protected virtual void OnDisconnected(DisconnectReason reason) { }
    }
}