using UnityEngine;

public class ThirdHole : MonoBehaviour
{
    public TriggerStuff triggerStuff;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Ball in the hole!");
            // Insert TriggerStuff method to call here
            triggerStuff.MiniGolfComplete();
        }
    }
}
