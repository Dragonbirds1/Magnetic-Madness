using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Player1Animator : MonoBehaviour
{
    public Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            animator.SetBool("IsRight1", true);
            animator.SetBool("IsForwardIdle", false);
            animator.SetBool("IsLeft1", false);
            animator.SetBool("IsUp1", false);
        }
        else if (Input.GetKeyUp(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
            {
                animator.SetBool("IsLeft1", true);
                animator.SetBool("IsForwardIdle", false);
                animator.SetBool("IsRight1", false);
                animator.SetBool("IsUp1", false);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool("IsRight1", false);
                animator.SetBool("IsForwardIdle", false);
                animator.SetBool("IsLeft1", false);
                animator.SetBool("IsUp1", true);
            }
            else
            {
                animator.SetBool("IsRight1", false);
                animator.SetBool("IsForwardIdle", true);
                animator.SetBool("IsLeft1", false);
                animator.SetBool("IsUp1", false);
            }
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.SetBool("IsLeft1", true);
            animator.SetBool("IsForwardIdle", false);
            animator.SetBool("IsRight1", false);
            animator.SetBool("IsUp1", false);
        }
        else if (Input.GetKeyUp(KeyCode.A))
        {
            if (Input.GetKey(KeyCode.D))
            {
                animator.SetBool("IsRight1", true);
                animator.SetBool("IsForwardIdle", false);
                animator.SetBool("IsLeft1", false);
                animator.SetBool("IsUp1", false);
            }
            else if (Input.GetKey(KeyCode.W))
            {
                animator.SetBool("IsRight1", false);
                animator.SetBool("IsForwardIdle", false);
                animator.SetBool("IsLeft1", false);
                animator.SetBool("IsUp1", true);
            }
            else
            {
                animator.SetBool("IsLeft1", false);
                animator.SetBool("IsForwardIdle", true);
                animator.SetBool("IsRight1", false);
                animator.SetBool("IsUp1", false);
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetBool("IsUp1", true);
            animator.SetBool("IsForwardIdle", false);
            animator.SetBool("IsRight1", false);
            animator.SetBool("IsLeft1", false);
        }
        else if (Input.GetKeyUp(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.D))
            {
                animator.SetBool("IsRight1", true);
                animator.SetBool("IsForwardIdle", false);
                animator.SetBool("IsLeft1", false);
                animator.SetBool("IsUp1", false);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                animator.SetBool("IsLeft1", true);
                animator.SetBool("IsForwardIdle", false);
                animator.SetBool("IsRight1", false);
                animator.SetBool("IsUp1", false);
            }
            else {
                animator.SetBool("IsRight1", false);
                animator.SetBool("IsForwardIdle", true);
                animator.SetBool("IsLeft1", false);
                animator.SetBool("IsUp1", false);
            }
        }
    }
}
