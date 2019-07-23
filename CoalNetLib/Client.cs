using System.Net;
using System.Net.Sockets;
using CoalNetLib.Internal;

namespace CoalNetLib
{
    public class Client : NetBase
    {
        private Connection _server;

        public Client(string ip, int port)
        {
            _server = new Connection(new IPEndPoint(IPAddress.Parse(ip), port), this, 0);
            
            var client = new UdpClient();
            client.Connect(IPAddress.Parse(ip), port);
            
            StartListening(client, port);
        }

        protected override void ProcessReceivedData(IPEndPoint sender, byte[] data)
        {
            if (!_server.Equals(sender)) // Received data from source other than server -> ignore
            {
                return;
            }

            object packet = Serializer.Deserialize(data);

            switch (packet)
            {
                case PacketConnectionChallenge challenge:
                    break;
                case PacketConnectionRejected rejected:
                    break;
                default:
                    InvokePacket(_server, packet);
                    break;
            }
        }
    }
}