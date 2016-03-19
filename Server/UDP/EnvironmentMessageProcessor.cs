using Common.Contracts;
using Common.Utility;

namespace NeuroServer.Udp
{
    public class EnvironmentMessageProcessor
    {
        private readonly ISerializer<Deltas> _serializer = new BinarySerializer();

        public void AttachToServer(UdpServer server)
        {
            server.OnProcess += bytes => _serializer.Serialize(new Deltas(5f, 0.1f));
        }
    }
}
