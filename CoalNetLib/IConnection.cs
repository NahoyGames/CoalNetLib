using System;
using System.Net;

namespace CoalNetLib
{
    /// <summary>
    /// A connection to a client/server which sends and receives data to/from the endpoint
    /// </summary>
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// The ip address of this connection
        /// </summary>
        IPAddress Address { get; }

        /// <summary>
        /// Is the client connected?
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Send data to this connection
        /// </summary>
        /// <param name="data">Byte stream to send</param>
        /// <param name="length">Size of the data</param>
        /// <param name="args">Additional implementer-specific arguments</param>
        void Send(byte[] data, int length, params object[] args);

        /// <summary>
        /// Listen for incoming traffic from this connection. Equivalent of an update function
        /// </summary>
        /// <param name="received">Received packets</param>
        void Listen(Action<byte[]> received);
    }
}