using System;

namespace NeuroServer.Udp
{
    public interface IServer : IDisposable
    {
        event Func<byte[], byte[]> OnProcess;
        void Start();
        void Stop();
    }
}