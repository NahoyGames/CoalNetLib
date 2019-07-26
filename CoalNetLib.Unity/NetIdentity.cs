using UnityEngine;

namespace CoalNetLib.Unity
{
    public class NetIdentity : MonoBehaviour
    {
        public ushort ObjectId { get; private set; }

        public void Init(ushort id)
        {
           ObjectId = id;
        }
    }
}