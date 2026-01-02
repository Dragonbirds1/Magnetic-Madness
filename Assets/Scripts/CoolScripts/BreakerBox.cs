using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BreakerBox : MonoBehaviour
{
    [Header("Sliders")]
    [Tooltip("Slider1")]
    public Slider slider1;
    [Tooltip("Slider2")]
    public Slider slider2;
    [Tooltip("Slider3")]
    public Slider slider3;
    [Tooltip("Slider4")]
    public Slider slider4;
    [Tooltip("Slider5")]
    public Slider slider5;

    [Header("Scripts")]
    [Tooltip("Reference to the SecurityCam script")]
    public SecurityCam securityCam;
    [Tooltip("Reference to the BoxEnemy script")]
    public BoxEnemy boxEnemy;
    [Tooltip("Reference to the HideAndSeekEnemy script")]
    public HideAndSeekEnemy hideAndSeekEnemy;
    [Tooltip("Reference to the Enemy3Move script")]
    public Enemy3Move enemy3Move;
    [Tooltip("Reference to the player movement script")]
    public PlayerMovement playerMovement;
    [Tooltip("Reference to the player2 movement script")]
    public PlayerMovement playerMovement2;

    [Header("Colliders")]
    [Tooltip("Reference to the player1 collider")]
    public CapsuleCollider2D player1Collider;
    [Tooltip("Reference to the player2 collider")]
    public CapsuleCollider2D player2Collider;

    [Header("GameObjects")]
    public GameObject JustDont;

    [Header("Bools")]
    public bool YOLO;
    public bool YOLO2;
    public bool YOLO3;
    public bool YOLO4;
    public bool YOLO5;

    [Header("Audio")]
    public AudioSource startUp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        JustDont.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (slider1.value == 1 && YOLO == false)
        {
            securityCam.isDisabled = true;
            securityCam.StopAudio();
            YOLO = true;
        }
        if (slider1.value == 0 && YOLO == true)
        {
            startUp.Play();
            securityCam.isDisabled = false;
            securityCam.audioSource.PlayOneShot(securityCam.patrolSFX);
            YOLO = false;
        }
        if (slider2.value == 1 && YOLO2 == false)
        {
            YOLO2 = true;
            boxEnemy.detectionRange = 0f;
            hideAndSeekEnemy.detectionRadius = 0f;
            hideAndSeekEnemy.viewDistance = 0f;
            enemy3Move.detectionRange = 0f;
        }
        else if (slider2.value == 0 && YOLO2 == true)
        {
            {
                YOLO2 = false;
                boxEnemy.detectionRange = 5f;
                hideAndSeekEnemy.detectionRadius = 4;
                hideAndSeekEnemy.viewDistance = 6f;
                enemy3Move.detectionRange = 5f;
            }
        }
        if (slider3.value == 1 && YOLO3 == false)
        {
            YOLO3 = true;
            playerMovement.moveSpeed = 10f;
            playerMovement2.moveSpeed = 10f;
        }
        else if (slider3.value == 0 && YOLO3 == true)
        {
            YOLO3 = false;
            playerMovement.moveSpeed = 5f;
            playerMovement2.moveSpeed = 5f;
        }
        if (slider4.value == 1 && YOLO4 == false)
        {
            YOLO4 = true;
            player1Collider.enabled = false;
            player2Collider.enabled = false;
        }
        else if (slider4.value == 0 && YOLO4 == true)
        {
            YOLO4 = false;
            player1Collider.enabled = true;
            player2Collider.enabled = true;
        }
        if (slider5.value == 1 && YOLO5 == false)
        {
            YOLO5 = true;
            JustDont.SetActive(true);
        }
        else if (slider5.value == 0 && YOLO5 == true)
        {
            YOLO5 = false;
            JustDont.SetActive(false);
        }
    }
}
