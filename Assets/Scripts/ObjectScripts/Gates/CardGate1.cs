using UnityEngine;

public class CardGate1 : MonoBehaviour
{
    [Header("Required CardReader Count")]
    [Tooltip("Number of card readers active to open the gate")]
    public int requiredCardReaderCount = 3;

    [Header("Current Active CardReader Count")]
    [Tooltip("Current number of active card readers")]
    public int currentActiveCardReaderCount = 0;

    [Header("Is Gate Locked")]
    [Tooltip("Is the gate currently locked?")]
    public bool isGateLocked = true;

    [Header("Gate GameObject")]
    [Tooltip("The Gate GameObject")]
    public GameObject gate;

    [Header("Gate Open Position")]
    [Tooltip("The position of the gate when open")]
    public Animator gateOpen;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentActiveCardReaderCount == requiredCardReaderCount && isGateLocked == true)
        {
            isGateLocked = false;
            gateOpen.SetBool("IsGate", true);
            Debug.Log("Gate Unlocked!");
        }
    }
}
