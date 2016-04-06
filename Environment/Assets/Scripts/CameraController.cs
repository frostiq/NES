using UnityEngine;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour
    {
        public GameObject player;

        private Vector3 offset;

        private Rigidbody playerRigidbody;

        // Use this for initialization
        void Start ()
        {
            offset = transform.position - player.transform.position;
            playerRigidbody = player.GetComponent<Rigidbody>();
        }
	
        // Update is called once per frame
        void LateUpdate ()
        {
            transform.position = player.transform.position + offset;
            transform.rotation = Quaternion.FromToRotation(Vector3.forward, playerRigidbody.velocity);
        }
    }
}
