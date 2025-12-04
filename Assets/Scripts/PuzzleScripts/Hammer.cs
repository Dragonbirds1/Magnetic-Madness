using Unity.VisualScripting;
using UnityEditor.Rendering;
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
    public GameObject Mole1, Mole2, Mole3, Mole4, Mole5, Mole6;
    public float hammerRange = 0.5f;
    public WackAMole wackAMole;

    void Start()
    {
        hit = Physics2D.Raycast(transform.position, Vector2.zero);
        rb = GetComponent<Rigidbody2D>();
        moleHit = mole.GetComponent<MoleHit>();
    }

    public void Update()
    {
        
        MousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        rb.linearVelocity = new Vector2((MousePos.x - transform.position.x) * hammerSpeed, (MousePos.y - transform.position.y) * hammerSpeed);

        if (Input.GetKeyDown(hitKey))
        {
            if (Vector2.Distance(transform.position, Mole1.transform.position) <= hammerRange && Mole1.activeSelf)
            {
                Mole1.SetActive(false);
                wackAMole.addScore();
            }

            if (Vector2.Distance(transform.position, Mole2.transform.position) <= hammerRange && Mole2.activeSelf)
            {
                Mole2.SetActive(false);
                wackAMole.addScore();
                //wackAMole.Hole2Timer = 0.0f;
            }

            if (Vector2.Distance(transform.position, Mole3.transform.position) <= hammerRange && Mole3.activeSelf)
            {
                Mole3.SetActive(false);
                wackAMole.addScore();
                //wackAMole.Hole3Timer = 0.0f;
            }

            if (Vector2.Distance(transform.position, Mole4.transform.position) <= hammerRange && Mole4.activeSelf)
            {
                Mole4.SetActive(false);
                wackAMole.addScore();
                //wackAMole.Hole4Timer = 0.0f;
            }

            if (Vector2.Distance(transform.position, Mole5.transform.position) <= hammerRange && Mole5.activeSelf)
            {
                Mole5.SetActive(false);
                wackAMole.addScore();
                //wackAMole.Hole5Timer = 0.0f;
            }

            if (Vector2.Distance(transform.position, Mole6.transform.position) <= hammerRange && Mole6.activeSelf)
            {
                Mole6.SetActive(false);
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
