using System.Net;
using System.Net.Sockets;
using Common.Contracts;
using Common.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Lib
{
    public class ServerGates
    {
		private readonly NetworkStream stream;
		private readonly ISerializer<Control> deserializer = new BinarySerializer();
		private Control control;

        public ServerGates()
        {
            var tcpClient = new TcpClient("localhost", 52200) {ReceiveTimeout = 500};
            stream = tcpClient.GetStream();
        }

        public void SendPicture(byte[] data)
        {
            stream.Write(data, 0, data.Length);
            var receiveBytes = new byte[9];
            stream.Read(receiveBytes, 0, receiveBytes.Length);
            control = deserializer.Deserialize(receiveBytes);
            Debug.Log(control.DeltaAngle);
        }

        public void UpdateAnimat(Rigidbody rigidbody)
        {
            rigidbody.velocity += control.DeltaVelocity *
                                  (rigidbody.velocity.sqrMagnitude == 0f
                                      ? Vector3.forward
                                      : rigidbody.velocity.normalized);
            rigidbody.velocity = Quaternion.Euler(0f, control.DeltaAngle * 5f, 0f) * rigidbody.velocity;

			if (control.Restart) {
				SceneManager.LoadScene (0);
			}
        }
    }
}