using UnityEngine;

public class Ballsy : MonoBehaviour
{
    [Header("Camera References")]
    [Tooltip("Reference to the bowling camera")]
    public GameObject camBowl;
    [Tooltip("Reference to the follow camera")]
    public GameObject camFollow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Spot"))
        {
            camBowl.SetActive(false);
            camFollow.SetActive(true);
        }
    }
}
