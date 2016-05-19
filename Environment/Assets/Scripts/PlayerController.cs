using System.Net;
using Assets.Scripts.Lib;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour
    {
        public Text CountText;
        public Text WinText;
        public Camera Eyes;

        private new Rigidbody rigidbody;
        private int _count;
        private ScreenshootMaker screenshootMaker;

		private static ServerGates serverGates = new ServerGates ();

        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.forward;
            Count = 0;
            screenshootMaker = new ScreenshootMaker(new Resolution { width = 256 + 32, height = 64 });
        }

        void FixedUpdate()
        {
            serverGates.UpdateAnimat(rigidbody);
        }

        void LateUpdate()
        {
            var screenshoot = screenshootMaker.TakeScreenshoot(Eyes);
			serverGates.UpdateScore (Count);
            serverGates.SendPicture(screenshoot);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Pick Up"))
            {
                other.gameObject.SetActive(false);
                ++Count;
            }
        }

        private int Count
        {
            get { return _count; }
            set
            {
                _count = value;

                CountText.text = "Count: " + value;

                if (value == 0)
                    WinText.text = string.Empty;
                else if (value >= 12)
                    WinText.text = "You Win!";
            }
        }
    }
}
