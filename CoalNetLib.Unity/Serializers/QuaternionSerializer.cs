using System;
using FurnaceSerializer;
using UnityEngine;

namespace CoalNetLib.Unity.Serializers
{
    public class QuaternionSerializer : ISerializer
    {
        public Type Type => typeof(Quaternion);

        public int SizeOf(object value) => sizeof(float) * 4;

        public bool Write(object value, byte[] buffer, ref int position) =>
            SerializerUtil.WriteFloat(((Quaternion) value).x, buffer, ref position)
            && SerializerUtil.WriteFloat(((Quaternion) value).y, buffer, ref position)
            && SerializerUtil.WriteFloat(((Quaternion) value).z, buffer, ref position)
            && SerializerUtil.WriteFloat(((Quaternion) value).w, buffer, ref position);

        public object Read(byte[] buffer, ref int position, bool peek = false)
        {
            float x = SerializerUtil.ReadFloat(buffer, ref position, peek);
            float y = SerializerUtil.ReadFloat(buffer, ref position, peek);
            float z = SerializerUtil.ReadFloat(buffer, ref position, peek);
            float w = SerializerUtil.ReadFloat(buffer, ref position, peek);
            
            return new Quaternion(x, y, z, w);
        }
    }
}