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
		public int SeriesNumber { get; private set; }

		private readonly TcpClient tcpClient;
		private readonly ISerializer<Request> reqSerializer = new BinarySerializer ();
		private readonly ISerializer<Response> respSerializer = new BinarySerializer ();

		Response response = new Response (Response.MessageType.Pause);
		float score;


		public ServerGates ()
		{
			tcpClient = new TcpClient () { ReceiveTimeout = 500 };
			SeriesNumber = 1;
		}

		public void SendPicture (byte[] data)
		{
			if (!tcpClient.Connected) {
				tcpClient.Connect ("localhost", 52200);
			}
			
		    var stream = tcpClient.GetStream ();
			Request request = new EmptyRequest ();

			switch (response.Type) {
			case Response.MessageType.Start:
				request = new ImageRequest (data);
				break;
			case Response.MessageType.Control:
				request = new ImageRequest (data);
				break;
			case Response.MessageType.Result:
				request = new ScoreRequest (score);
				break;
			case Response.MessageType.Finish:
				SceneManager.LoadScene (0);
				SeriesNumber++;
				break;
			case Response.MessageType.End:
				Application.Quit ();
				break;
			default:
				break;
			}

			var reqBytes = reqSerializer.Serialize (request);
			stream.Write (reqBytes, 0, reqBytes.Length);
			var receiveBytes = new byte[9];
			stream.Read (receiveBytes, 0, receiveBytes.Length);
			response = respSerializer.Deserialize (receiveBytes);
			Debug.Log (string.Format("{0:0.000} {1:0.000}", response.DeltaAngle, response.DeltaVelocity));
		}

		public void UpdateAnimat (Rigidbody rigidbody)
		{
			rigidbody.velocity += response.DeltaVelocity *
			(rigidbody.velocity.sqrMagnitude == 0f
                                      ? Vector3.forward
                                      : rigidbody.velocity.normalized);
			rigidbody.velocity = Quaternion.Euler (0f, response.DeltaAngle * 5f, 0f) * rigidbody.velocity;
		}

		public void UpdateScore (float score)
		{
			this.score = score;
		}
	}
}