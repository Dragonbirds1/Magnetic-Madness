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
    public float triggerValue;
    event System.Action onMoleWhacked;
    bool isMoleHole1Occupied, isMoleHole2Occupied, isMoleHole3Occupied, isMoleHole4Occupied, isMoleHole5Occupied, isMoleHole6Occupied;
    bool isMoleAbleToSpawn = false;
    public GameObject currentMoleInHole1, currentMoleInHole2, currentMoleInHole3, currentMoleInHole4, currentMoleInHole5, currentMoleInHole6;
    public float Hole1Timer, Hole2Timer, Hole3Timer, Hole4Timer, Hole5Timer, Hole6Timer;
    bool startHole1Timer, startHole2Timer, startHole3Timer, startHole4Timer, startHole5Timer, startHole6Timer;
    public int randomMole;
    public float waitTimeBetweenMoles;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isMoleHole1Occupied = false;
        isMoleHole2Occupied = false;
        isMoleHole3Occupied = false;
        isMoleHole4Occupied = false;
        isMoleHole5Occupied = false;
        isMoleHole6Occupied = false;
        startHole1Timer = false;
        startHole2Timer = false;
        startHole3Timer = false;
        startHole4Timer = false;
        startHole5Timer = false;
        startHole6Timer = false;
        currentMoleInHole1.SetActive(false);
        currentMoleInHole2.SetActive(false);
        currentMoleInHole3.SetActive(false);
        currentMoleInHole4.SetActive(false);
        currentMoleInHole5.SetActive(false);
        currentMoleInHole6.SetActive(false);
        triggerValue = Gamepad.current.rightTrigger.ReadValue();
        gamePopUp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoleAbleToSpawn)
        {
            
            waitTimeBetweenMoles += Time.deltaTime;
            if (waitTimeBetweenMoles >= 3.0f)
            {
                CreateMole();
                waitTimeBetweenMoles = 0.0f;
                
            }
        }
        else
        {
            return;
        }
        if (startHole1Timer)
        {
            Hole1Timer += Time.deltaTime;
        }
        if (isGameActive)
        {
           
        }
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
        isMoleAbleToSpawn = true;
        isGameActive = true;
        timer = 0.0f;
        score = 0;
        InvokeRepeating(nameof(SpawnMole), 0.0f, spawnInterval);
        wackAMoleGame.SetActive(true);
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
        if (isMoleHole1Occupied)
        {
            return;
        }
        

        if (randomMole == 1)
        {
            currentMoleInHole1.SetActive(true);
            isMoleHole1Occupied = true;
            startHole1Timer = true;
        }

        if (Hole1Timer >= 3.0f)
        {
            currentMoleInHole1.SetActive(false);
            isMoleHole1Occupied = false;
            startHole1Timer = false;
            Hole1Timer = 0.0f;
        }

    }
    
    void MoleWhacked()
    {
        score++;
        if (onMoleWhacked != null)
        {
            onMoleWhacked();
        }
    }

    void CreateMole()
    {
        randomMole = Random.Range(1, 6);
        isMoleAbleToSpawn = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, rangeToStartGame);

    }
}

