using System;
using FurnaceSerializer;
using UnityEngine;

namespace CoalNetLib.Unity.Serializers
{
    public class Vector4Serializer : ISerializer
    {
        public Type Type => typeof(Vector4);

        public int SizeOf(object value) => sizeof(float) * 4;

        public bool Write(object value, byte[] buffer, ref int position) =>
            SerializerUtil.WriteFloat(((Vector4) value).x, buffer, ref position)
            && SerializerUtil.WriteFloat(((Vector4) value).y, buffer, ref position)
            && SerializerUtil.WriteFloat(((Vector4) value).z, buffer, ref position)
            && SerializerUtil.WriteFloat(((Vector4) value).w, buffer, ref position);

        public object Read(byte[] buffer, ref int position, bool peek = false)
        {
            float x = SerializerUtil.ReadFloat(buffer, ref position, peek);
            float y = SerializerUtil.ReadFloat(buffer, ref position, peek);
            float z = SerializerUtil.ReadFloat(buffer, ref position, peek);
            float w = SerializerUtil.ReadFloat(buffer, ref position, peek);
            
            return new Vector4(x, y, z, w);
        }
    }
}