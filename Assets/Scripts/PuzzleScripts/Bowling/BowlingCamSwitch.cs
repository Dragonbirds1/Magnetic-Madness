using UnityEngine;

public class BowlingCamSwitch : MonoBehaviour
{
    public GameObject camBowl;
    public GameObject camFollow;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("BowlingBall")) return;

        if (camBowl) camBowl.SetActive(false);
        if (camFollow) camFollow.SetActive(true);
    }
}
