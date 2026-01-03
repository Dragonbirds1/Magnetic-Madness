using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hammer : MonoBehaviour
{
    Rigidbody2D rb;
    public float hammerSpeed = 10f;
    public Vector2 MousePos;
    public KeyCode hitKey;
    public GameObject mole;
    RaycastHit2D hit;
    MoleHit moleHit;
    public bool hasHit = false;
    public Animator button1, button2, button3, button4, button5, button6;
    public GameObject Mole1, Mole2, Mole3, Mole4, Mole5, Mole6;
    public float hammerRange = 0.5f;
    public WackAMole wackAMole;
    bool button1Flash, button2Flash, button3Flash, button4Flash, button5Flash, button6Flash;

    void Start()
    {
        hit = Physics2D.Raycast(transform.position, Vector2.zero);
        rb = GetComponent<Rigidbody2D>();
        moleHit = mole.GetComponent<MoleHit>();
    }

    public void Update()
    {
        button1Flash = button1.GetBool("IsFlash");
        button2Flash = button2.GetBool("IsFlash");
        button3Flash = button3.GetBool("IsFlash");
        button4Flash = button4.GetBool("IsFlash");
        button5Flash = button5.GetBool("IsFlash");
        button6Flash = button6.GetBool("IsFlash");

        MousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        rb.linearVelocity = new Vector2((MousePos.x - transform.position.x) * hammerSpeed, (MousePos.y - transform.position.y) * hammerSpeed);

        if (Input.GetKeyDown(hitKey))
        {
            if (Vector2.Distance(transform.position, Mole1.transform.position) <= hammerRange && button1Flash == true)
            {
                button1.SetBool("IsFlash", false);
                wackAMole.addScore();
            }

            if (Vector2.Distance(transform.position, Mole2.transform.position) <= hammerRange && button2Flash == true)
            {
                button2.SetBool("IsFlash", false);
                wackAMole.addScore();
                //wackAMole.Hole2Timer = 0.0f;
            }

            if (Vector2.Distance(transform.position, Mole3.transform.position) <= hammerRange && button3Flash == true)
            {
                button3.SetBool("IsFlash", false);
                wackAMole.addScore();
                //wackAMole.Hole3Timer = 0.0f;
            }

            if (Vector2.Distance(transform.position, Mole4.transform.position) <= hammerRange && button4Flash == true)
            {
                button4.SetBool("IsFlash", false);
                wackAMole.addScore();
                //wackAMole.Hole4Timer = 0.0f;
            }

            if (Vector2.Distance(transform.position, Mole5.transform.position) <= hammerRange && button5Flash == true)
            {
                button5.SetBool("IsFlash", false);
                wackAMole.addScore();
                //wackAMole.Hole5Timer = 0.0f;
            }

            if (Vector2.Distance(transform.position, Mole6.transform.position) <= hammerRange && button6Flash == true)
            {
                button6.SetBool("IsFlash", false);
                wackAMole.addScore();
                //wackAMole.Hole6Timer = 0.0f;
            }
        }
    }

    void CheckHit()
    {
        hit = Physics2D.Raycast(transform.position, Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Mole"))
        {
            HasHit();
            moleHit = hit.collider.gameObject.GetComponent<MoleHit>();
            Debug.Log("Mole Hit Confirmed!");
        }
    }

    void HasHit()
    {
        moleHit.GetHit();
    }

   

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hammerRange);
    }
}
