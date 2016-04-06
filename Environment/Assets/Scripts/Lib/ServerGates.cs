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
        private readonly ISerializer<Deltas> _deserializer = new BinarySerializer();
        private IPEndPoint _fakeEndPoint = new IPEndPoint(IPAddress.Any, 0);
        private Deltas _deltas;

        public ServerGates(IPEndPoint serverEndpoint, int listenPort)
        {
            _udp = new UdpClient(listenPort);
            _udp.Connect(serverEndpoint);
            _udp.Client.ReceiveTimeout = 500;
        }

        public void SendPicture(byte[] data)
        {
            _udp.Send(data, data.Length);
            _udp.BeginReceive(asyncResult =>
            {
                var u = (UdpClient) asyncResult.AsyncState;
                var receiveBytes = u.EndReceive(asyncResult, ref _fakeEndPoint);
                _deltas = _deserializer.Deserialize(receiveBytes);
            }, _udp);
        }

        public void UpdateAnimat(Rigidbody rigidbody)
        {
            rigidbody.velocity += _deltas.DeltaVelocity * (rigidbody.velocity.sqrMagnitude == 0f ? Vector3.forward : rigidbody.velocity.normalized);
            rigidbody.velocity = Quaternion.Euler(Vector3.up*_deltas.DeltaAngle) * rigidbody.velocity;
        }
    }
}