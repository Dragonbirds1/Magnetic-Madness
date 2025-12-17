using System.Runtime;
using UnityEngine;

public class MiniGolf : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public Transform arrow;
    public GameObject player;
    public GameObject golfCam1;

    [Header("Power Settings")]
    public float powerMultiplier = 5f;
    public float maxPower = 15f;

    [Header("Ball Settings")]
    public float stopThreshold = 0.05f;
    public float radToHit;

    private Vector2 dragStart;
    private bool isAiming;
    bool camMove = false;

    void Update()
    {
        if (camMove)
        {
            golfCam1.SetActive(true);
        }
        else if (!camMove)
        {
            golfCam1.SetActive(false);
        }
        float playerPos = Vector2.Distance(transform.position, player.transform.position);
        if (playerPos <= radToHit)
        {
                // Ball must be stopped
            if (rb.linearVelocity.magnitude > stopThreshold)
            {
                arrow.gameObject.SetActive(false);
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                dragStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                isAiming = true;
                arrow.gameObject.SetActive(true);
            }

            if (Input.GetMouseButton(0) && isAiming)
            {
                UpdateArrow();
            }

            if (Input.GetMouseButtonUp(0) && isAiming)
            {
                camMove = true;
                ShootBall();
                isAiming = false;
                arrow.gameObject.SetActive(false);
            }

        }
        else if (playerPos > radToHit)
        {
            return;
        } 
    }

    void UpdateArrow()
    {
        Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = dragStart - currentPos;

        float power = Mathf.Clamp(direction.magnitude * powerMultiplier, 0, maxPower);

        // Position arrow on ball
        arrow.position = transform.position;

        // Rotate arrow toward shot direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        arrow.rotation = Quaternion.Euler(0, 0, angle);

        // Scale arrow to show force
        arrow.localScale = new Vector3(1f, power, 1f);
    }

    void ShootBall()
    {
        Vector2 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 forceDir = (dragStart - endPos);

        float power = Mathf.Clamp(forceDir.magnitude * powerMultiplier, 0, maxPower);
        rb.AddForce(forceDir.normalized * power, ForceMode2D.Impulse);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radToHit);
    }
}
