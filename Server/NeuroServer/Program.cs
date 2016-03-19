using System;
using NeuroServer.Udp;

namespace NeuroServer
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var server = new UdpServer(52200))
            {
                server.Start();
                new EnvironmentMessageProcessor().AttachToServer(server);
                Console.Read();
            }
        }
    }
}
