using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float Speed;
    public Text CountText;
    public Text WinText;

    private Rigidbody Rigidbody { get; set; }

    private int _count;


    void Start()
    {
        Rigidbody = GetComponent<Rigidbody>();
        Count = 0;
    }

    void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        var movement = new Vector3(moveHorizontal, 0f, moveVertical);

        Rigidbody.AddForce(movement * Speed);
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
