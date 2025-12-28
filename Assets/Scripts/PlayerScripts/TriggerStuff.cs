using UnityEngine;

public class TriggerStuff : MonoBehaviour
{
    [Header("Trigger Box GameObjects")]
    [Tooltip("Trigger box for bowling")]
    public GameObject bowlingCompleteTrigger;
    [Tooltip("Trigger box for miniGolf")]
    public GameObject miniGolfCompleteTrigger;
    [Tooltip("Trigger box for WackAMole")]
    public GameObject wackAMoleCompleteTrigger;
    [Tooltip("Trigger box for hiding spots")]
    public GameObject hidingSpotTrigger;


    [Header("Item GameObjects")]
    [Tooltip("Keycard GameObject")]
    public GameObject keycard;
    [Tooltip("Keycard2 GameObject")]
    public GameObject keycard2;
    [Tooltip("Keycard3 GameObject")]
    public GameObject keycard3;

    [Header("Boolean Settings")]
    [Tooltip("Can claim card?")]
    public bool canClaimCard = false;
    [Tooltip("Can claim card2?")]
    public bool canClaimCard2 = false;
    [Tooltip("Can claim card3?")]
    public bool canClaimCard3 = false;
    [Tooltip("Is hiding spot triggered?")]
    public bool isHidingSpotTriggered = false;

    [Header("Script Information")]
    [Tooltip("Script Name: HideAndSeekEnemy.cs")]
    public HideAndSeekEnemy hideAndSeekEnemy;
    [Tooltip("Script Name: MainSecurityCam.cs")]
    public MainSecurityCam mainSecurityCam;
    [Tooltip("Script Name: SecurityCam.cs")]
    public SecurityCam securityCam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //keycard.SetActive(false);
       // keycard2.SetActive(false);
       // keycard3.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BowlingTriggerBox"))
        {
            BowlingComplete();
        }
        if (other.CompareTag("MiniGolfTriggerBox"))
        {
            MiniGolfComplete();
        }
        if (other.CompareTag("WackAMoleTriggerBox"))
        {
            WackAMoleComplete();
        }
        if (other.CompareTag("Keycard"))
        {
            canClaimCard = true;
            keycard.SetActive(false);
            Debug.Log("Picked up the Keycard!");
            // Additional actions for keycard pickup can be added here
        }
        if (other.CompareTag("Keycard2"))
        {
            canClaimCard2 = true;
            keycard2.SetActive(false);
            Debug.Log("Picked up the Keycard2!");
            // Additional actions for keycard2 pickup can be added here
        }
        if (other.CompareTag("Keycard3"))
        {
            canClaimCard3 = true;
            keycard3.SetActive(false);
            Debug.Log("Picked up the Keycard3!");
            // Additional actions for keycard3 pickup can be added here
        }
        if (other.CompareTag("DetectionForCam")) 
        { 
            //securityCam.isActive = true;
        }
    }

    void BowlingComplete()
    {
        bowlingCompleteTrigger.SetActive(false);
        Debug.Log("Bowling Complete Trigger Activated");
        Debug.Log("Completed Bowling, GOOD JOB :D");
        keycard2.SetActive(true);
    }

    void MiniGolfComplete()
    {
        miniGolfCompleteTrigger.SetActive(false);
        Debug.Log("MiniGolf Complete Trigger Activated");
        Debug.Log("Completed MiniGolf, GOOD JOB :D");
        keycard.SetActive(true);
    }

    void WackAMoleComplete()
    {
        wackAMoleCompleteTrigger.SetActive(false);
        Debug.Log("WackAMole Complete Trigger Activated");
        Debug.Log("Completed WackAMole, GOOD JOB :D");
        Debug.Log("BRO REALLY USED THE TRIGGER BOX FOR WACKAMOLE???");
        keycard3.SetActive(true);
    }
}
