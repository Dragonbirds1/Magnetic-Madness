using UnityEngine;
using UnityEngine.UI;

public class MovementSwap : MonoBehaviour
{
    public KeyCode key;
    public GameObject movement1;
    public GameObject movement2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement2.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (key == KeyCode.K)
        {
            movement2.SetActive(true);
            movement1.SetActive(false);
        }
    }
}
