using System;
using System.Net;

namespace CoalNetLib
{
    /// <summary>
    /// Listens for incoming connections and reports them to the high level API
    /// </summary>
    public interface IListener
    {
        /// <summary>
        /// Begin listening for incoming connection requests.
        /// </summary>
        /// <param name="port">Beginning listening for incoming connections on the port</param>
        void BeginListen(int port);

        /// <summary>
        /// Get the next packet, if any
        /// </summary>
        /// <param name="sender">Sender of the data</param>
        /// <param name="data">Raw data received</param>
        /// <returns>Whether the listener actually received a packet or not</returns>
        bool GetNextPacket(out IPEndPoint sender, out byte[] data);

        /// <summary>
        /// Send a packet 
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="data"></param>
        /// <param name="length"></param>
        void SendPacket(IPEndPoint receiver, byte[] data, int length = -1);
    }
}