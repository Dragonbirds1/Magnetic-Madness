using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BowlingPin : MonoBehaviour
{
    [Header("Knock Detection")]
    public float knockMoveDistance = 0.25f;
    public float knockRotateDegrees = 25f;
    public float knockSpeed = 2.0f; // if it gets blasted, count it

    [HideInInspector] public bool isKnockedOver;
    [HideInInspector] public bool alreadyScored;

    private Vector3 startPos;
    private float startZ;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CacheStart();
    }

    void CacheStart()
    {
        startPos = transform.position;
        startZ = transform.eulerAngles.z;
    }

    void Update()
    {
        if (isKnockedOver) return;

        float moved = Vector2.Distance(transform.position, startPos);
        float rot = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, startZ));

#if UNITY_6000_0_OR_NEWER
        float speed = rb.linearVelocity.magnitude;
#else
        float speed = rb.velocity.magnitude;
#endif

        if (moved > knockMoveDistance || rot > knockRotateDegrees || speed > knockSpeed)
            isKnockedOver = true;
    }

    public void ResetPin(bool recacheStart = false)
    {
        if (recacheStart) CacheStart();

        transform.position = startPos;
        transform.rotation = Quaternion.Euler(0, 0, startZ);

#if UNITY_6000_0_OR_NEWER
        rb.linearVelocity = Vector2.zero;
#else
        rb.velocity = Vector2.zero;
#endif
        rb.angularVelocity = 0f;

        isKnockedOver = false;
    }

    public void ResetForNewFrame()
    {
        gameObject.SetActive(true);
        alreadyScored = false;
        ResetPin(recacheStart: false);
    }

    public bool IsStanding()
    {
        return gameObject.activeInHierarchy && !isKnockedOver;
    }

}
