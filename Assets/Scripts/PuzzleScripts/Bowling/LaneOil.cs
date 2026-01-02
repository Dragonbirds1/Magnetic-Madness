using UnityEngine;

public class LaneOil : MonoBehaviour
{
    [Range(0f, 5f)]
    public float extraLinearDrag = 1.0f; // higher = more slow-down in oil

    void OnTriggerStay2D(Collider2D col)
    {
        if (!col.CompareTag("BowlingBall")) return;

        var rb = col.attachedRigidbody;
        if (!rb) return;

        // Multiplicative damping that is stable across framerate
        float damp = Mathf.Exp(-extraLinearDrag * Time.fixedDeltaTime);

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity *= damp;
#else
        rb.velocity *= damp;
#endif
    }
}
