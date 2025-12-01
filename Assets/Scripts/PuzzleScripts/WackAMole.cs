using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class WackAMole: MonoBehaviour
{
    public GameObject player, player2;
    public KeyCode keyboardStartGameKey = KeyCode.Space;
    public KeyCode ControllerStartGameKey = KeyCode.JoystickButton3;
    public KeyCode keyboardHammerKey = KeyCode.Mouse0;
    bool isAtGame = false;
    public float rangeToStartGame = 2.0f;
    public float moleVisibleDuration = 1.5f;
    public float gameDuration = 30.0f;
    bool isGameActive = false;
    public GameObject hammerPrefab;
    public GameObject wackAMoleGame;
    public GameObject molePrefab;
    public Transform[] spawnPoints;
    public float spawnInterval = 2.0f;
    private float timer;
    public int score;
    public GameObject gamePopUp;
    public float triggerValue = Gamepad.current.rightTrigger.ReadValue();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        gamePopUp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(player.transform.position, transform.position) <= rangeToStartGame || Vector3.Distance(player2.transform.position, transform.position) <= rangeToStartGame)
        {
            isAtGame = true;
            if (!isGameActive && (Input.GetKeyDown(keyboardStartGameKey) || Input.GetKeyDown(ControllerStartGameKey))) // Gamepad.current.rightTrigger.isPressed
            {
                //StartGame();
                gamePopUp.SetActive(true);
            }
        }
        else
        {
            isAtGame = false;
        }
    }

    public void StartGame()
    {
        isGameActive = true;
        timer = 0.0f;
        score = 0;
        InvokeRepeating(nameof(SpawnMole), 0.0f, spawnInterval);
        wackAMoleGame.SetActive(true);
        Instantiate(hammerPrefab);
    }
    public void EndGame()
    {
        isGameActive = false;
        CancelInvoke(nameof(SpawnMole));
        wackAMoleGame.SetActive(false);
        // Display final score or any end game UI here
    }

    void SpawnMole()
    {
        if (!isGameActive) return;
        int spawnIndex = Random.Range(0, spawnPoints.Length);
        Transform spawnPoint = spawnPoints[spawnIndex];
        Instantiate(molePrefab, spawnPoint.position, spawnPoint.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, rangeToStartGame);

    }
}

