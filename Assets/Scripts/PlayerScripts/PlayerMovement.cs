using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    public KeyCode up, down, left, right;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (Input.GetKey(up))
        {
            rb.linearVelocityY = moveSpeed;
        }
        else if (Input.GetKey(down))
        {
            rb.linearVelocityY = -moveSpeed;
        }
        else if (Input.GetKey(left))
        {
            rb.linearVelocityX = -moveSpeed;
        }
        else if (Input.GetKey(right))
        {
            rb.linearVelocityX = moveSpeed;
        }
        else
        {
            rb.linearVelocity = Vector2.zero; // Stop movement if arrow keys are pressed
        }
    }
}