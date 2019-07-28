using System;
using UnityEditor;
using UnityEngine;

namespace CoalNetLib.Unity.Editor
{
    [CustomEditor(typeof(Client))]
    public class ClientEditor : UnityEditor.Editor
    {
        private Client _client;

        private void OnEnable()
        {
            _client = (Client) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!EditorApplication.isPlaying) return;
            
            if (!_client.Connected)
            {
                if (GUILayout.Button("Connect"))
                {
                    _client.Connect();
                }
            }
            else if (_client.Connecting)
            {
                GUILayout.Button("Connecting...");
            }
            else
            {
                if (GUILayout.Button("Disconnect"))
                {
                    _client.Disconnect();
                }
            }
        }
    }
}