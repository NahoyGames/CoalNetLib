using System;
using FurnaceSerializer;
using UnityEngine;

namespace CoalNetLib.Unity.Serializers
{
    public class Vector3Serializer : ISerializer
    {
        public Type Type => typeof(Vector3);

        public int SizeOf(object value) => sizeof(float) * 3;

        public bool Write(object value, byte[] buffer, ref int position) =>
            SerializerUtil.WriteFloat(((Vector3) value).x, buffer, ref position)
            && SerializerUtil.WriteFloat(((Vector3) value).y, buffer, ref position)
            && SerializerUtil.WriteFloat(((Vector3) value).z, buffer, ref position);

        public object Read(byte[] buffer, ref int position, bool peek = false)
        {
            float x = SerializerUtil.ReadFloat(buffer, ref position, peek);
            float y = SerializerUtil.ReadFloat(buffer, ref position, peek);
            float z = SerializerUtil.ReadFloat(buffer, ref position, peek);
            
            return new Vector3(x, y, z);
        }
    }
}