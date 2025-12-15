using UnityEngine;

public class BowlingPin : MonoBehaviour
{
    public bool isKnockedOver;
    public bool alreadyScored;

    Vector3 startPos;
    Quaternion startRot;

    public float knockDistance = 0.3f;

    void Awake()
    {
        startPos = transform.position;
        startRot = transform.rotation;
    }

    void Update()
    {
        if (!isKnockedOver &&
            Vector3.Distance(transform.position, startPos) > knockDistance)
        {
            isKnockedOver = true;
        }
    }

    public void ResetPin()
    {
        transform.position = startPos;
        transform.rotation = startRot;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0;
        }

        isKnockedOver = false;
    }

    public void ResetForNewFrame()
    {
        gameObject.SetActive(true);
        alreadyScored = false;
        ResetPin();
    }
}
