using UnityEngine;

public class LaneOil : MonoBehaviour
{
    public float oilStrength = 0.5f;

    void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("BowlingBall"))
        {
            Rigidbody2D rb = col.GetComponent<Rigidbody2D>();
            rb.linearVelocity *= (1f - oilStrength * Time.fixedDeltaTime);
        }
    }
}
