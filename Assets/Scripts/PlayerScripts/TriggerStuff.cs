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
    [Tooltip("Trigger box for golf and bowling")]
    public GameObject golfAndBowlingTrigger;
    [Tooltip("Trigger box for MainMusic on")]
    public GameObject mainMusicTrigger;

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

    [Header("Animator Settings")]
    [Tooltip("Animator: DeadBox1")]
    public Animator deadBox1;
    [Tooltip("Animator: DeadBox2")]
    public Animator deadBox2;
    [Tooltip("Animator: KeycardMove")]
    public Animator keycardMove;
    [Tooltip("Animator: KeycardMove2")]
    public Animator keycardMove2;

    [Header("Audios")]
    [Tooltip("Audiosource: MainMusic")]
    public AudioSource mainMusic;
    [Tooltip("Audiosource: GameAreaMusic")]
    public AudioSource gameAreaMusic;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        keycard.SetActive(false);
        keycard2.SetActive(false);
        keycard3.SetActive(false);
        mainMusicTrigger.SetActive(false);
        gameAreaMusic.Stop();
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
        if (other.CompareTag("TouchBlock")) 
        { 
            mainMusic.Stop();
            gameAreaMusic.Play();
            golfAndBowlingTrigger.SetActive(false);
            mainMusicTrigger.SetActive(true);
        }
        if (other.CompareTag("MainMusicTriggerBox")) 
        { 
            mainMusic.Play();
            gameAreaMusic.Stop();
            mainMusicTrigger.SetActive(false);
            golfAndBowlingTrigger.SetActive(true);
        }
    }

    public void BowlingComplete()
    {
        bowlingCompleteTrigger.SetActive(false);
        Debug.Log("Bowling Complete Trigger Activated");
        Debug.Log("Completed Bowling, GOOD JOB :D");
        keycard2.SetActive(true);
        keycardMove2.SetBool("MOVE", true);
    }

    public void MiniGolfComplete()
    {
        miniGolfCompleteTrigger.SetActive(false);
        Debug.Log("MiniGolf Complete Trigger Activated");
        Debug.Log("Completed MiniGolf, GOOD JOB :D");
        keycard.SetActive(true);
    }

    public void WackAMoleComplete()
    {
        wackAMoleCompleteTrigger.SetActive(false);
        Debug.Log("WackAMole Complete Trigger Activated");
        Debug.Log("Completed WackAMole, GOOD JOB :D");
        Debug.Log("BRO REALLY USED THE TRIGGER BOX FOR WACKAMOLE???");
        keycard3.SetActive(true);
        deadBox1.SetBool("MOVE", true);
        deadBox2.SetBool("MOVE", true);
        keycardMove.SetBool("MOVE", true);
    }
}
