using System;
using System.Net;
using System.Net.Sockets;
using Common.Contracts;
using Common.Utility;
using UnityEngine;

namespace Assets.Scripts.Lib
{
    public class ServerGates
    {
        private readonly UdpClient _udp;
        private readonly IPEndPoint _serverEndPoint;
        private readonly ISerializer<Deltas> _deserializer = new BinarySerializer();
        private IPEndPoint _fakeEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private IAsyncResult _asyncResult = null;

        public ServerGates(IPEndPoint serverEndpoint, int listenPort)
        {
            _serverEndPoint = serverEndpoint;
            _udp = new UdpClient(listenPort);
            _udp.Connect(serverEndpoint);
            _udp.Client.ReceiveTimeout = 1000;
        }

        public void SendPicture(byte[] data)
        {
            _udp.Send(data, data.Length);
            _asyncResult = _udp.BeginReceive(null, null);
        }

        public void UpdateAnimat(Rigidbody rigidbody)
        {
            if (_asyncResult != null && _asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(500)))
            {
                var bytes = _udp.EndReceive(_asyncResult, ref _fakeEndPoint);
                if(_fakeEndPoint.Equals(_serverEndPoint))
                   UpdateAnimat(rigidbody, bytes);
                _asyncResult = null;
            }
        }

        private void UpdateAnimat(Rigidbody rigidbody, byte[] bytes)
        {
            var deltas = _deserializer.Deserialize(bytes);
            rigidbody.MoveRotation(Quaternion.Euler(Vector3.up * deltas.DeltaAngle));
            rigidbody.velocity *= deltas.DeltaVelocity;
        }
    }
}
