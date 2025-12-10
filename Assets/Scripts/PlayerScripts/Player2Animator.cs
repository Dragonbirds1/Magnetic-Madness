using UnityEngine;
using UnityEngine.InputSystem;

public class Player2Animator : MonoBehaviour
{
    public Animator animator;
    [SerializeField] private float deadZone = 0.2f;

    void Update()
    {
        Debug.Log("Current State: " + animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));

        if (Gamepad.current == null)
            return;

        Vector2 stick = Gamepad.current.leftStick.ReadValue();

        bool isMoving = stick.magnitude > deadZone;
        animator.SetBool("IsMoving", isMoving);

        if (isMoving)
        {
            Vector2 dir = stick.normalized;

            animator.SetFloat("MoveX", dir.x);
            animator.SetFloat("MoveY", dir.y);
        }
        else
        {
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", 0f);
        }
    }
}
