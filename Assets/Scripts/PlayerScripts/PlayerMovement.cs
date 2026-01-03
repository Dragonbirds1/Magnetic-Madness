using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;

    private Vector2 movement;

    public WackAMole wackAMole;

    public ToggleCutscene toggleCutscene;

    public Player1Death player1Death;

    public Player2Death player2Death;

    public bool player1IsMoving = true;
    public bool player2IsMoving = true;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        if (wackAMole.freezeGameplay == false || player1Death.isDead == false || player2Death.isDead == false || toggleCutscene.isCutsceneActive == false || player1IsMoving == true || player2IsMoving == true)
        {
            movement = ctx.ReadValue<Vector2>();
        }
        else if (wackAMole.freezeGameplay == true || player1Death.isDead == true || player2Death.isDead == false || toggleCutscene.isCutsceneActive == true || player1IsMoving == false || player2IsMoving == false)
        {
            return;
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement * moveSpeed;
    }
}