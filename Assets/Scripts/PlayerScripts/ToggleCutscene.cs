using System;
using UnityEngine;
using TMPro;


public class ToggleCutscene : MonoBehaviour
{   
    [Header("Enemy Scripts")]
    [Tooltip("Reference to the HideAndSeekEnemy script")]
    public HideAndSeekEnemy hideAndSeekEnemy;

    [Header("Cutscene")]
    [Tooltip("Animator controlling the cutscene")]
    public Animator cutsceneAnimator;

    [Header("Cutscene Cameras")]
    [Tooltip("Cameras involved in the cutscene")]
    public GameObject cutsceneCam;

    [Header("Player Cameras")]
    [Tooltip("Cameras attached to the players")]
    public GameObject player1Cam, player2Cam;

    [Header("Cutscene 1 Time")]
    [Tooltip("Duration of the cutscene")]
    public float cutscene1Time = 0f;

    [Header("Cutscene 2 Time")]
    [Tooltip("Duration of the second cutscene")]
    public float cutscene2Time = 0f;

    [Header("Toggle Block 1")]
    [Tooltip("Object that triggers the cutscene")]
    public GameObject toggleBlock1;

    [Header("Toggle Block 2")]
    [Tooltip("Object that triggers the second cutscene")]
    public GameObject toggleBlock2;

    [Header("Timer Text")]
    [Tooltip("UI Text element displaying the timer")]
    public TMP_Text timerText;
    
    [Header("Countdown Time")]
    [Tooltip("Time for countdown before enemy activation")]
    public float countdownTime = 120f;
    private float initialCountdownTime;

    [Header("Time Display GameObject")]
    [Tooltip("GameObject that holds the time display UI")]
    public GameObject timeDisplayGameObject;

    [Header("UI Crap")]
    [Tooltip("The seperator for both cameras/Boarder")]
    public GameObject boarder;
    [Tooltip("The mouse UI for player two")]
    public GameObject mouseUI;

    [Header("Audiosources")]
    [Tooltip("Audio source for main music")]
    public AudioSource mainMusic;
    [Tooltip("Audio source for hide music")]
    public AudioSource hideMusic;

    string currentTimeText;
    bool cutscene1TimeActive = false;
    bool cutscene2TimeActive = false;
    bool startCountdown = false;
    bool sendOutTheBeast = false;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toggleBlock2.SetActive(false);
        cutsceneCam.SetActive(false);
        timeDisplayGameObject.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        if (startCountdown)
        {
            initialCountdownTime = Mathf.Max(0, countdownTime);
            countdownTime -= Time.deltaTime;
            timeDisplayGameObject.SetActive(true);
            int minutes = Mathf.FloorToInt(countdownTime / 60);
            int seconds = Mathf.FloorToInt(countdownTime % 60);
            currentTimeText = ("Time Left: " + string.Format("{0:00}:{1:00}", minutes, seconds));
            timerText.text = currentTimeText;
            if (countdownTime <= 0)
            {
                countdownTime = 0;
                timeDisplayGameObject.SetActive(false);
                startCountdown = false;
                sendOutTheBeast = true;
            }
        }
        if (sendOutTheBeast)
        {
            hideAndSeekEnemy.isMoving = true;
            sendOutTheBeast = false;
        }
        if (cutscene1TimeActive)
        {
            cutscene1Time += Time.deltaTime;
            if (cutscene1Time >= 18.333f)
            {
                toggleBlock2.SetActive(true);
                mainMusic.UnPause();
                boarder.SetActive(true);
                mouseUI.SetActive(true);
                Cursor.visible = true;
                cutsceneCam.SetActive(false);
                player1Cam.SetActive(true);
                player2Cam.SetActive(true);
                cutscene1Time = 0f;
                cutscene1TimeActive = false;
            }
        }
        if (cutscene2TimeActive)
        {
            cutscene2Time += Time.deltaTime;
            if (cutscene2Time >= 38.750f)
            {
                boarder.SetActive(true);
                mouseUI.SetActive(true);
                Cursor.visible = true;
                cutsceneCam.SetActive(false);
                player1Cam.SetActive(true);
                player2Cam.SetActive(true);
                cutscene2Time = 0f;
                cutscene2TimeActive = false;
                startCountdown = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ToggleMe"))
        {
            mainMusic.Pause();
            boarder.SetActive(false);
            mouseUI.SetActive(false);
            Cursor.visible = false;
            cutsceneCam.SetActive(true);
            player1Cam.SetActive(false);
            player2Cam.SetActive(false);
            cutsceneAnimator.SetBool("StartCutscene", true);
            cutscene1TimeActive = true;
            toggleBlock1.SetActive(false);
        }

        if (collision.CompareTag("ToggleMe2"))
        {
            mainMusic.Pause();
            boarder.SetActive(false);
            mouseUI.SetActive(false);
            Cursor.visible = false;
            cutsceneCam.SetActive(true);
            player1Cam.SetActive(false);
            player2Cam.SetActive(false);
            cutsceneAnimator.SetBool("MoveOn", true);
            cutscene2TimeActive = true;
            toggleBlock2.SetActive(false);
        }

        if (collision.CompareTag("ToggleMe3"))
        {
            countdownTime = 0f;
        }
    }
}
