using System;
using UnityEditor;
using UnityEngine;

namespace CoalNetLib.Unity.Editor
{
    [CustomEditor(typeof(NetConfig))]
    public class NetConfigEditor : UnityEditor.Editor
    {
        private SerializedProperty _port, _timeout, _serializers, _packetScripts,
            _maxConnections, _updateRate, _serverMaxPacketSize, _clientMaxPacketSize;

        private Vector2 _packetScriptsScroll;
        
        private void OnEnable()
        {
            _port = serializedObject.FindProperty("port");
            _timeout = serializedObject.FindProperty("timeout");
            _serializers = serializedObject.FindProperty("serializers");
            _packetScripts = serializedObject.FindProperty("packetScripts");
            _maxConnections = serializedObject.FindProperty("maxConnections");
            _updateRate = serializedObject.FindProperty("updateRate");
            _serverMaxPacketSize = serializedObject.FindProperty("serverMaxPacketSize");
            _clientMaxPacketSize = serializedObject.FindProperty("clientMaxPacketSize");
            
            _packetScriptsScroll = new Vector2();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // Draw inspector
            EditorGUILayout.LabelField("Server", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_port);
            EditorGUILayout.PropertyField(_timeout);
            EditorGUILayout.PropertyField(_maxConnections);
            EditorGUILayout.PropertyField(_updateRate);
            EditorGUILayout.PropertyField(_serverMaxPacketSize);
            
            EditorGUILayout.LabelField("Client", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_clientMaxPacketSize);
            
            EditorGUILayout.LabelField("Packets", EditorStyles.boldLabel);
            EditorGUILayout.BeginScrollView(_packetScriptsScroll);
            for (int i = 0; i < _packetScripts.arraySize; i++)
            {
                EditorGUILayout.PropertyField(_packetScripts.GetArrayElementAtIndex(i));
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("+"))
            {
                _packetScripts.arraySize++;
            }

            if (GUILayout.Button("-"))
            {
                _packetScripts.arraySize--;
            }
            EditorGUILayout.EndHorizontal();
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}