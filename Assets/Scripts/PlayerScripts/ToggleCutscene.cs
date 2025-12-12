using UnityEngine;

public class ToggleCutscene : MonoBehaviour
{   
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

    bool cutscene1TimeActive = false;
    bool cutscene2TimeActive = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cutsceneCam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (cutscene1TimeActive)
        {
            cutscene1Time += Time.deltaTime;
            if (cutscene1Time >= 18.333f)
            {
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
            if (cutscene2Time >= 33.333f)
            {
                cutsceneCam.SetActive(false);
                player1Cam.SetActive(true);
                player2Cam.SetActive(true);
                cutscene2Time = 0f;
                cutscene2TimeActive = false;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ToggleMe"))
        {
            cutsceneCam.SetActive(true);
            player1Cam.SetActive(false);
            player2Cam.SetActive(false);
            cutsceneAnimator.SetBool("StartCutscene", true);
            cutscene1TimeActive = true;
            toggleBlock1.SetActive(false);
        }

        if (collision.CompareTag("ToggleMe2"))
        {
            cutsceneCam.SetActive(true);
            player1Cam.SetActive(false);
            player2Cam.SetActive(false);
            cutsceneAnimator.SetBool("MoveOn", true);
            cutscene2TimeActive = true;
            toggleBlock2.SetActive(false);
        }
    }
}
