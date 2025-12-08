using UnityEngine;

public class ToggleKeypad : MonoBehaviour
{
    public GameObject keypadPanel;
    public GameObject player2;
    public KeyCode toggleKey;
    public float toggleRadius = 2.0f;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keypadPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector2.Distance(transform.position, player2.transform.position);
        if (distance <= toggleRadius)
        {
            if (Input.GetKeyDown(toggleKey))
            {
                keypadPanel.SetActive(true);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, toggleRadius);
    }
}
