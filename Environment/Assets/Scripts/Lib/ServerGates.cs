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
		private readonly ISerializer<Request> reqSerializer = new BinarySerializer();
		private readonly ISerializer<Response> respSerializer = new BinarySerializer();

		Response response = new Response(Response.MessageType.Pause);
		float score;


        public ServerGates()
        {
            var tcpClient = new TcpClient("localhost", 52200) {ReceiveTimeout = 500};
            stream = tcpClient.GetStream();
        }

        public void SendPicture(byte[] data)
        {
			Request request;
			switch (response.Type) {
			case Response.MessageType.Control:
				request = new ImageRequest (data);
				break;
			case Response.MessageType.Restart:
				request = new ScoreRequest (score);
				break;
			case Response.MessageType.End:
				Application.Quit ();
				request = new EmptyRequest ();
				break;
			default:
				request = new EmptyRequest ();
				break;
			}


			var reqBytes = reqSerializer.Serialize(request);
			stream.Write(reqBytes, 0, reqBytes.Length);
            var receiveBytes = new byte[9];
            stream.Read(receiveBytes, 0, receiveBytes.Length);
			response = respSerializer.Deserialize(receiveBytes);
            Debug.Log(response.DeltaAngle);
        }

        public void UpdateAnimat(Rigidbody rigidbody)
        {
            rigidbody.velocity += response.DeltaVelocity *
                                  (rigidbody.velocity.sqrMagnitude == 0f
                                      ? Vector3.forward
                                      : rigidbody.velocity.normalized);
            rigidbody.velocity = Quaternion.Euler(0f, response.DeltaAngle * 5f, 0f) * rigidbody.velocity;

			if (response.Type == Response.MessageType.Restart) {
				SceneManager.LoadScene (0);
			}
        }

		public void UpdateScore(float score){
			this.score = score;
		}
    }
}