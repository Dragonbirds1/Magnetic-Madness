using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    public KeyCode key;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        float verticalMove = Input.GetAxisRaw("Vertical");

        // Check if the input is coming from arrow keys
        bool isArrowKeyInput = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) ||
                               Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);

            if (!isArrowKeyInput) // Only allow movement if it's not from arrow keys
            {
                rb.linearVelocity = new Vector2(horizontalMove * moveSpeed, verticalMove * moveSpeed);
            }
            else
            {
                rb.linearVelocity = Vector2.zero; // Stop movement if arrow keys are pressed
        }
    }
}