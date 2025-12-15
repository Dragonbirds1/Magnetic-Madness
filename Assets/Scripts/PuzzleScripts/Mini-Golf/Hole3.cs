using UnityEngine;

public class Hole3 : MonoBehaviour
{
    [Header("References")]
    public Rigidbody2D rb;
    public Transform arrow;

    [Header("Power Settings")]
    public float powerMultiplier = 5f;
    public float maxPower = 15f;

    [Header("Ball Settings")]
    public float stopThreshold = 0.05f;

    private Vector2 dragStart;
    private bool isAiming;

    void Update()
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
            ShootBall();
            isAiming = false;
            arrow.gameObject.SetActive(false);
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
}
