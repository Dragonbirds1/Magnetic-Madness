using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    private Vector2 movement;

    public WackAMole wackAMole;

    public Player1Death player1Death;

    public Player2Death player2Death;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        if (wackAMole.freezeGameplay == false || player1Death.isDead == false || player2Death.isDead == false)
        {
            movement = ctx.ReadValue<Vector2>();
        }
        else if (wackAMole.freezeGameplay == true || player1Death.isDead == true || player2Death.isDead == false)
        {
            return;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }
}