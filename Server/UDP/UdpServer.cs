using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace NeuroServer.Udp
{
    public class UdpServer: IDisposable
    {
        public event Func<byte[], byte[]> OnProcess;

        private UdpClient _udp;
        private Task _listenTask;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ILogger _log = LogManager.GetLogger("Server");

        public void Init(int port)
        {
            _udp = new UdpClient(port);
            var token = _cancellationTokenSource.Token;
            _listenTask = new Task(() => Listen(token), token);
        }

        public void Start()
        {
            _listenTask.Start();
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        public void Dispose()
        {
            Stop();
            _udp.Dispose();
        }

        private void Listen(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    IPEndPoint endPoint = null;
                    var request = _udp.Receive(ref endPoint);
                    _log.Info("Message recieved");

                    var response = OnProcess?.Invoke(request);

                    if (response != null)
                        _udp.Send(response, response.Length, endPoint);

                    _log.Info("Message sent");
                }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
            finally
            {
                _udp.Close();
            }
        }
    }
}
