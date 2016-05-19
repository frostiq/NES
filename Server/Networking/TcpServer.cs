using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace NeuroServer.Udp
{
    public class TcpServer : IServer
    {
        public event Func<byte[], byte[]> OnProcess;

        private readonly TcpListener _tcpListener;
        private readonly ILogger _log = LogManager.GetLogger("Server");

        private Task _listenTask;
        private CancellationTokenSource _cancellationTokenSource;


        public TcpServer(int port)
        {
            _tcpListener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            _tcpListener.Start();
            _log.Info("Server started");

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;
            _listenTask = Task.Factory.StartNew(() => Listen(token), token);
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
            _tcpListener.Stop();
            _log.Info("Server stopped");
        }

        public void Dispose()
        {
            Stop();
        }

        private void Listen(CancellationToken token)
        {
            _log.Info("Waiting for connection...");

            while (true)
            {
                token.ThrowIfCancellationRequested();

                var client = _tcpListener.AcceptTcpClient();

                _log.Info($"Connection accepted form {client.Client.RemoteEndPoint}");

                Task.Factory.StartNew(() => Dispatch(client, token), token);
            }
        }

        private void Dispatch(TcpClient client, CancellationToken token)
        {
            try
            {
                var bytes = new byte[65536];

                using (var stream = client.GetStream())
                    while (true)
                    {
                        token.ThrowIfCancellationRequested();

                        stream.Read(bytes, 0, bytes.Length);
                        _log.Debug("Message recieved");

                        var response = OnProcess?.Invoke(bytes);

                        if (response != null)
                            stream.Write(response, 0, response.Length);

                        _log.Debug("Message sent");
                    }
            }
            catch (Exception e)
            {
                _log.Error(e);
            }
            finally
            {
                client.Close();
            }

        }
    }
}
