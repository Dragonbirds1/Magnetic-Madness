using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
//using UnityEngine.UIElements;

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
    public TMP_Text Score;
    public TMP_Text YourScore, YourScore2, ScoreNeeded, ScoreNeeded2;
    public GameObject ScoreBoard, TMP;
    bool SpawnText = false;
    public float TimeBetweenTextSpawn = 0.0f;


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
        if (Vector3.Distance(player.transform.position, transform.position) <= rangeToStartGame || Vector3.Distance(player2.transform.position, transform.position) <= rangeToStartGame)
        {
            isAtGame = true;
            if (isAtGame && (Input.GetKeyDown(keyboardStartGameKey) || Input.GetKeyDown(ControllerStartGameKey))) // Gamepad.current.rightTrigger.isPressed
            {
                Debug.Log("Wack A Mole Pop Up Activated");
                //StartGame();
                gamePopUp.SetActive(true);
            }
        }
        else
        {
            isAtGame = false;
        }
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
            if (Hole1Timer >= 3.0f)
            {
                currentMoleInHole1.SetActive(false);
                isMoleHole1Occupied = false;
                startHole1Timer = false;
                Hole1Timer = 0.0f;
            }
        }

        if (startHole2Timer)
        {
            Hole2Timer += Time.deltaTime;
            if (Hole2Timer >= 3.0f)
            {
                currentMoleInHole2.SetActive(false);
                isMoleHole2Occupied = false;
                startHole2Timer = false;
                Hole2Timer = 0.0f;
            }
        }

        if (startHole3Timer)
        {
            Hole3Timer += Time.deltaTime;
            if (Hole3Timer >= 3.0f)
            {
                currentMoleInHole3.SetActive(false);
                isMoleHole3Occupied = false;
                startHole3Timer = false;
                Hole3Timer = 0.0f;
            }
        }

        if (startHole4Timer)
        {
            Hole4Timer += Time.deltaTime;
            if (Hole4Timer >= 3.0f)
            {
                currentMoleInHole4.SetActive(false);
                isMoleHole4Occupied = false;
                startHole4Timer = false;
                Hole4Timer = 0.0f;
            }
        }

        if (startHole5Timer)
        {
            Hole5Timer += Time.deltaTime;
            if (Hole5Timer >= 3.0f)
            {
                currentMoleInHole5.SetActive(false);
                isMoleHole5Occupied = false;
                startHole5Timer = false;
                Hole5Timer = 0.0f;
            }
        }

        if (startHole6Timer)
        {
            Hole6Timer += Time.deltaTime;
            if (Hole6Timer >= 3.0f)
            {
                currentMoleInHole6.SetActive(false);
                isMoleHole6Occupied = false;
                startHole6Timer = false;
                Hole6Timer = 0.0f;
            }
        }

        

        if (isGameActive)
        {
            Score.text = "Score: " + score.ToString();

            gameDuration -= Time.deltaTime;

            if (gameDuration <= 0.0f)
            {
                EndGame();
                
            }
        }

        if (SpawnText)
        {
            TimeBetweenTextSpawn += Time.deltaTime;
            if (TimeBetweenTextSpawn >= 0.5f)
            {
                YourScore.gameObject.SetActive(true);
            }
            if (TimeBetweenTextSpawn >= 1.0f)
            {
                YourScore2.gameObject.SetActive(true);
            }
            if (TimeBetweenTextSpawn >= 1.5f)
            {
                ScoreNeeded.gameObject.SetActive(true);
            }
            if (TimeBetweenTextSpawn >= 2.0f)
            {
                ScoreNeeded2.gameObject.SetActive(true);
                SpawnText = false;
                TimeBetweenTextSpawn = 0.0f;
            }
        }

    }

    public void StartGame()
    {
        Cursor.visible = false;
        isMoleAbleToSpawn = true;
        isGameActive = true;
        timer = 0.0f;
        score = 0;
        InvokeRepeating(nameof(SpawnMole), 0.0f, spawnInterval);
        wackAMoleGame.SetActive(true);
    }
    public void EndGame()
    {
        SpawnText = true;
        isGameActive = false;
        isMoleAbleToSpawn = false;
        CancelInvoke(nameof(SpawnMole));
        //wackAMoleGame.SetActive(false);
        
        TMP.SetActive(true);
        ScoreBoard.SetActive(true);

        YourScore2.text = score.ToString();

        
        if (score >= 10)
        {
            Debug.Log("Wack A Mole Completed!");
            // Trigger puzzle completion logic here
        }
        else
        {
            Debug.Log("Wack A Mole Failed. Score: " + score);
        }
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

        if (randomMole == 2)
        {
            currentMoleInHole2.SetActive(true);
            isMoleHole2Occupied = true;
            startHole2Timer = true;
        }

        if (randomMole == 3)
        {
            currentMoleInHole3.SetActive(true);
            isMoleHole3Occupied = true;
            startHole3Timer = true;
        }

        if (randomMole == 4)
        {
            currentMoleInHole4.SetActive(true);
            isMoleHole4Occupied = true;
            startHole4Timer = true;
        }

        if (randomMole == 5)
        {
            currentMoleInHole5.SetActive(true);
            isMoleHole5Occupied = true;
            startHole5Timer = true;
        }

        if (randomMole == 6)
        {
            currentMoleInHole6.SetActive(true);
            isMoleHole6Occupied = true;
            startHole6Timer = true;
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

    public void addScore()
    {
        score++;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, rangeToStartGame);

    }
}

