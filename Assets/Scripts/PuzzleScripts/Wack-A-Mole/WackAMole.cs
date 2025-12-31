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
    public KeyCode LeaveWackAMoleKey;
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
    public GameObject YourSc, YourSc2, ScoreNe, ScoreNe2, Win, Loose;
    public GameObject ScoreBoard, TMP;
    bool SpawnText;
    public float TimeBetweenTextSpawn;
    public bool Playing = false;
    public bool AddScore = false;
    public GameObject Music, Drumroll;
    public GameObject Camera;
    public GameObject Player1Cam, Player2Cam;
    bool letsGoBack = false;
    public GameObject Boarder;
    bool canToggleGame = true;
    public bool freezeGameplay = false;
    public Animator buttonAnim1, buttonAnim2, buttonAnim3, buttonAnim4, buttonAnim5, buttonAnim6;
    public TriggerStuff triggerStuff;
    bool YAY = false;
    bool BOO = false;
    public AudioSource mainMusic;
    public KeyCode closeWackAMolePopup;


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
        triggerValue = Gamepad.current.rightTrigger.ReadValue();
        gamePopUp.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (letsGoBack)
        {
            if (Input.GetKeyDown(LeaveWackAMoleKey))
            {
                mainMusic.UnPause();
                freezeGameplay = false;
                score = 0;
                canToggleGame = true;
                Cursor.visible = true;
                Camera.SetActive(false);
                Player1Cam.SetActive(true);
                Player2Cam.SetActive(true);
                YourSc.SetActive(false);
                YourSc2.SetActive(false);
                ScoreNe.SetActive(false);
                ScoreNe2.SetActive(false);
                TMP.SetActive(false);
                ScoreBoard.SetActive(false);
                Win.SetActive(false);
                Loose.SetActive(false);
                hammerPrefab.SetActive(false);
                Boarder.SetActive(true);
                letsGoBack = false;
                buttonAnim1.SetBool("IsFlash", false);
                buttonAnim2.SetBool("IsFlash", false);
                buttonAnim3.SetBool("IsFlash", false);
                buttonAnim4.SetBool("IsFlash", false);
                buttonAnim5.SetBool("IsFlash", false);
                buttonAnim6.SetBool("IsFlash", false);
                if (YAY == true && BOO == false)
                {
                    triggerStuff.WackAMoleComplete();
                    YAY = false;
                    BOO = false;
                }
                else if (BOO == true && YAY == false)
                {
                    YAY = false;
                    BOO = false;
                    return;
                }
            }
        }

        if (canToggleGame)
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
                else if (Input.GetKeyDown(closeWackAMolePopup))
                {
                    Debug.Log("Wack A Mole Pop Up Deactivated");
                    gamePopUp.SetActive(false);
                }
            }
            else if (Vector3.Distance(player.transform.position, transform.position) > rangeToStartGame)
            {
                isAtGame = false;
                gamePopUp.SetActive(false);
            }
        }
        else if (canToggleGame == false)
        {
            isAtGame = false;
        }

        if (SpawnText)
        {
            TimeBetweenTextSpawn += Time.deltaTime;

            if (TimeBetweenTextSpawn >= 1.0f)
            {
                YourSc.SetActive(true);
            }
            if (TimeBetweenTextSpawn >= 2.0f)
            {
                YourSc2.SetActive(true);
            }
            if (TimeBetweenTextSpawn >= 3.0f)
            {
                ScoreNe.SetActive(true);
            }
            if (TimeBetweenTextSpawn >= 4.0f)
            {
                ScoreNe2.SetActive(true);
            }
            if (TimeBetweenTextSpawn >= 5.4f)
            {
                SpawnText = false;
                TimeBetweenTextSpawn = 0.0f;
                letsGoBack = true;
                if (score >= 10)
                {
                    Winner();
                    YAY = true;
                    BOO = false;
                }
                else if (score < 10)
                {
                    Looser();
                    BOO = true;
                    YAY = false;
                }
            }
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
                buttonAnim1.SetBool("IsFlash", false);
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
                buttonAnim2.SetBool("IsFlash", false);
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
                buttonAnim3.SetBool("IsFlash", false);
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
                buttonAnim4.SetBool("IsFlash", false);
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
                buttonAnim5.SetBool("IsFlash", false);
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
                buttonAnim6.SetBool("IsFlash", false);
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
                gameDuration = 30.0f;
                EndGame();
                
            }
        }

        

    }

    public void StartGame()
    {
        mainMusic.Pause();
        YAY = false;
        BOO = false;
        freezeGameplay = true;
        canToggleGame = false;
        Playing = true;
        Music.SetActive(true);
        Drumroll.SetActive(false);
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
        waitTimeBetweenMoles = 0.0f;
        
        Playing = false;
        Music.SetActive(false);
        Drumroll.SetActive(true);
        SpawnText = true;
        isGameActive = false;
        isMoleAbleToSpawn = false;
        CancelInvoke(nameof(SpawnMole));
        //wackAMoleGame.SetActive(false);
        
        TMP.SetActive(true);
        ScoreBoard.SetActive(true);

        YourScore2.text = score.ToString();

        buttonAnim1.SetBool("IsFlash", false);
        buttonAnim2.SetBool("IsFlash", false);
        buttonAnim3.SetBool("IsFlash", false);
        buttonAnim4.SetBool("IsFlash", false);
        buttonAnim5.SetBool("IsFlash", false);
        buttonAnim6.SetBool("IsFlash", false);


        //if (score >= 10)
        //{
        // Debug.Log("Wack A Mole Completed!");
        //Trigger puzzle completion logic here
        //}
        //else
        //{
        //Debug.Log("Wack A Mole Failed. Score: " + score);
        //}
    }

    void SpawnMole()
    {
        if (isMoleHole1Occupied)
        {
            return;
        }
        

        if (randomMole == 1)
        {
            buttonAnim1.SetBool("IsFlash", true);
            isMoleHole1Occupied = true;
            startHole1Timer = true;
        }

        if (randomMole == 2)
        {
            buttonAnim2.SetBool("IsFlash", true);
            isMoleHole2Occupied = true;
            startHole2Timer = true;
        }

        if (randomMole == 3)
        {
            buttonAnim3.SetBool("IsFlash", true);
            isMoleHole3Occupied = true;
            startHole3Timer = true;
        }

        if (randomMole == 4)
        {
            buttonAnim4.SetBool("IsFlash", true);
            isMoleHole4Occupied = true;
            startHole4Timer = true;
        }

        if (randomMole == 5)
        {
            buttonAnim5.SetBool("IsFlash", true);
            isMoleHole5Occupied = true;
            startHole5Timer = true;
        }

        if (randomMole == 6)
        {
            buttonAnim6.SetBool("IsFlash", true);
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
        if (Playing == true)
        {
            score++;
        }
        else if (Playing == false)
        {
            return;
        }
    }

    public void Winner()
    {
        Debug.Log("Wack A Mole Completed!");
        Win.SetActive(true);
    }

    public void Looser()
    {
        Debug.Log("Wack A Mole Failed. Score: " + score);
        Loose.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, rangeToStartGame);

    }
}

