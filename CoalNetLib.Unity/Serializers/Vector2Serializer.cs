using System;
using FurnaceSerializer;
using UnityEngine;

namespace CoalNetLib.Unity.Serializers
{
    public class Vector2Serializer : ISerializer
    {
        public Type Type => typeof(Vector2);

        public int SizeOf(object value) => sizeof(float) * 2;

        public bool Write(object value, byte[] buffer, ref int position) =>
            SerializerUtil.WriteFloat(((Vector2) value).x, buffer, ref position)
            && SerializerUtil.WriteFloat(((Vector2) value).y, buffer, ref position);
        
        public object Read(byte[] buffer, ref int position, bool peek = false)
        {
            float x = SerializerUtil.ReadFloat(buffer, ref position, peek);
            float y = SerializerUtil.ReadFloat(buffer, ref position, peek);
            
            return new Vector2(x, y);
        }
    }
}