using UnityEngine;

public class Player1Win : MonoBehaviour
{
    [Header("Player1 Reference")]
    public GameObject player1;

    [Header("Exit Door Reference")]
    public GameObject exitDoor;

    [Header("AbilitySwap Script")]
    public AbilitySwap abilitySwap;

    [Header("PlayerAbility Script")]
    public PlayerAbility playerAbility;

    [Header("PlayerMovement Script")]
    public PlayerMovement playerMovement;

    [Header("Player1Cam")]
    public GameObject cam1;

    [Header("Wait Reference")]
    public GameObject wait;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Ensure wait object is inactive at start
        wait.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("ExitDoor"))
        {
            // Activate wait object
            wait.SetActive(true);
            // Disable AbilitySwap script
            abilitySwap.enabled = false;
            // Move camera to different parent
            cam1.transform.parent = exitDoor.transform;
            abilitySwap.death = true;
            player1.SetActive(false);
            Debug.Log("Player 1 Wins!");
            Debug.Log("Waiting for Player 2 to reach the exit...");
        }
    }
}
