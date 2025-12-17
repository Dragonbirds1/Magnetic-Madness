using UnityEngine;

public class HideAndSeekEnemy : MonoBehaviour
{
    [Header("Hide and Seek Enemy GameObject")]
    [Tooltip("The Hide and Seek Enemy GameObject")]
    public GameObject hideAndSeekEnemy;

    [Header("Player GameObject")]
    [Tooltip("The Player GameObject")]
    public GameObject player;

    [Header("Waypoints for Hide and Seek Enemy")]
    [Tooltip("Array of waypoints for the Hide and Seek Enemy to navigate")]
    public Transform[] waypoints;

    [Header("Movement Speed")]
    [Tooltip("Speed at which the Hide and Seek Enemy moves")]
    public float movementSpeed = 2f;

    [Header("Voice Line Audio Source")]
    [Tooltip("Audio source for the Hide and Seek Enemy's voice lines")]
    public AudioSource voiceLineAudioSource;
    public AudioSource chaseVoiceLineAudioSource;

    [Header("Voice Line Clips")]
    [Tooltip("Array of audio clips for the Hide and Seek Enemy's voice lines")]
    public AudioClip[] voiceLineClips;
    public AudioClip chaseVoiceLineClip;

    [Header("Time Between Waypoints")]
    [Tooltip("Time delay between reaching one waypoint and moving to the next")]
    public float timeBetweenWaypoints = 4f;

    [Header("Raduis to Detect Player")]
    [Tooltip("Radius within which the enemy can detect the player")]
    public float detectionRadius;

    private int currentWaypointIndex = 0;
    
    public bool isMoving = false;
    bool isWaiting = false;
    bool startTimer = false;
    bool PLAYONCE = false;
    bool runPlayerRun = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        
        if (isMoving)
        {
            hideAndSeekEnemy.transform.position = Vector3.MoveTowards(hideAndSeekEnemy.transform.position, waypoints[currentWaypointIndex].position, movementSpeed * Time.deltaTime);
            float playerDistance = Vector3.Distance(transform.position, player.transform.position);

            if (playerDistance <= detectionRadius && PLAYONCE == false)
            {
                PLAYONCE = true;
                playSFX();
                runPlayerRun = true;
            }
            else
            {
                isWaiting = false;
                startTimer = false;
                timeBetweenWaypoints = 4f;
            }
            if (runPlayerRun)
            {
                hideAndSeekEnemy.transform.position = Vector3.MoveTowards(hideAndSeekEnemy.transform.position, player.transform.position, movementSpeed * 2 * Time.deltaTime);
            }
            if (Vector3.Distance(hideAndSeekEnemy.transform.position, waypoints[currentWaypointIndex].position) <= 0.1f)
            {
                timeBetweenWaypoints -= Time.deltaTime;
                if (timeBetweenWaypoints <= 0f)
                {
                    PlayRandomVoiceLine();
                    currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
                    timeBetweenWaypoints = 4f;
                }
            }
        }
    }
    
    void PlayRandomVoiceLine()
    {
        if (voiceLineClips.Length == 0) return;

        int randomIndex = Random.Range(0, 3);
        voiceLineAudioSource.clip = voiceLineClips[randomIndex];
        voiceLineAudioSource.Play();
    }

    void playSFX()
    {
        chaseVoiceLineAudioSource.PlayOneShot(chaseVoiceLineClip);
        return;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}