using UnityEngine;

public class BoxOnPlate : MonoBehaviour
{
    [Header("Plate Reference")]
    public GameObject plate;

    [Header("Gate References")]
    public GameObject Gate1;
    public GameObject Gate2;
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
        if (other.gameObject.CompareTag("Plate"))
        {
            Debug.Log("Box on Plate");
            Gate1.SetActive(false);
            Gate2.SetActive(false);
        }
    }
}
