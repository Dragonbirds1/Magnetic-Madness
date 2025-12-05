using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    private Vector2 movement;

    public WackAMole wackAMole;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        if (wackAMole.freezeGameplay == false)
        {
            movement = ctx.ReadValue<Vector2>();
        }
        else if (wackAMole.freezeGameplay == true)
        {
            return;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }
}