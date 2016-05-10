using System.Net;
using System.Net.Sockets;
using Common.Contracts;
using Common.Utility;
using UnityEngine;

namespace Assets.Scripts.Lib
{
    public class ServerGates
    {
        private readonly NetworkStream _stream;
        private readonly ISerializer<Deltas> _deserializer = new BinarySerializer();
        private Deltas _deltas;

        public ServerGates(IPEndPoint serverEndpoint, int listenPort)
        {
            var tcpClient = new TcpClient("localhost", 52200) {ReceiveTimeout = 500};
            _stream = tcpClient.GetStream();
        }

        public void SendPicture(byte[] data)
        {
            _stream.Write(data, 0, data.Length);
            var receiveBytes = new byte[65536];
            _stream.Read(receiveBytes, 0, receiveBytes.Length);
            _deltas = _deserializer.Deserialize(receiveBytes);
            Debug.Log(_deltas.DeltaAngle);
        }

        public void UpdateAnimat(Rigidbody rigidbody)
        {
            rigidbody.velocity += _deltas.DeltaVelocity *
                                  (rigidbody.velocity.sqrMagnitude == 0f
                                      ? Vector3.forward
                                      : rigidbody.velocity.normalized);
            rigidbody.velocity = Quaternion.Euler(0f, _deltas.DeltaAngle * 5f, 0f) * rigidbody.velocity;
        }
    }
}