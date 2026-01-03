using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class AIEnemyMovement : MonoBehaviour
{
    public float speed = 3.0f;
    public float waitTime = 1;
    public float chaseTime = 5;
    public List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    private Rigidbody2D rb;
    private Vector3 currentPosition;
    private Vector3 currentVelocity;
    private Vector3 currentRotation;
    private Vector3 currentRotationDirection;
    private Vector3 currentVelocityDirection;
    private bool playerFound = false, player2Found = false;
    private bool isChasing = false;
    public GameObject player, player2;
    public float chaseSpeed = 5.0f;
    public float detectionRange = 5.0f;
    bool patrol = true;
    bool patrol2 = true;
    bool patrol3 = true;
    public float randomAction;
    public float sleepTime = 3.0f;
    bool sleepTimeChange = false;
    public GameObject sleepEye, sleepEye2;
    public float timeTillDash = 2.0f;
    public float dashTime = 1.0f;
    public WackAMole wackAMole;






    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        randomAction = Random.Range(0f, 10f);
        transform.position = waypoints[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        if (wackAMole.freezeGameplay == false)
        {
            if (sleepTimeChange)
            {
                sleepTime -= Time.deltaTime;
            }
            else if (!sleepTimeChange)
            {
                patrol = true;
            }
            if (sleepTime <= 0)
            {
                sleepTimeChange = false;
                sleepEye.SetActive(false);
                sleepEye2.SetActive(false);
                randomAction = Random.Range(0f, 10f);
                sleepTime = 3.0f;
            }
            if (patrol2 == false)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, chaseSpeed * Time.deltaTime);
                dash();
                chaseTime -= Time.deltaTime;
            }
            if (patrol3 == false)
            {
                transform.position = Vector3.MoveTowards(transform.position, player2.transform.position, chaseSpeed * Time.deltaTime);
                dash();
                chaseTime -= Time.deltaTime;
            }
            if (chaseTime <= 0)
            {
                isChasing = false;
                patrol = true;
                patrol2 = true;
                patrol3 = true;
                chaseTime = 5;
            }
            if (patrol)
            {
                transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, speed * Time.deltaTime);
                if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
                {
                    if (randomAction <= 5f)
                    {
                        sleeping();

                    }
                    else if (randomAction > 5f && randomAction <= 10f)
                    {
                        waitTime -= Time.deltaTime;
                        if (waitTime <= 0)
                        {
                            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
                            Search();
                            Change();
                            waitTime = 3;
                        }
                    }
                }
            }
        }
        else if (wackAMole.freezeGameplay == true)
        {
            return;
        }
    }
    void Search() // Search for player within detection range
    {
        Debug.Log("Searching for player");
        if (Vector3.Distance(transform.position, player.transform.position) <= detectionRange)
        {
            playerFound = true;
        }
        else
        {
            playerFound = false;
        }
        if (Vector3.Distance(transform.position, player2.transform.position) <= detectionRange)
        {
            player2Found = true;
        }
        else
        {
            player2Found = false;
        }
        if (!playerFound)
        {
            Debug.Log("Player not found, continuing patrol");
            patrol = true;

        }
        else if (playerFound)
        {
            Debug.Log("Player found, engaging");
            isChasing = true;
            if (isChasing)
            {
                patrol = false;
                patrol2 = false;
                patrol3 = true;
            }

        }
        if (!player2Found)
        {
            Debug.Log("Player2 not found, continuing patrol");
            patrol = true;
        }
        else if (player2Found)
        {
            Debug.Log("Player2 found, engaging");
            isChasing = true;
            if (isChasing)
            {
                patrol = false;
                patrol2 = true;
                patrol3 = false;
            }
        }
    }

    void Change() // Change random action after each waypoint
    {
        randomAction = Random.Range(0f, 10f);
        return;
    }

    void sleeping() // Enemy sleep mechanic
    {
        sleepEye.SetActive(true);
        sleepEye2.SetActive(true);
        Debug.Log("Enemy is sleeping");
        patrol = false;
        sleepTimeChange = true;
        if (sleepTime <= 0)
        {
            patrol = true;
            
        }
    }

    void dash() // Dash mechanic for enemy when chasing player
    {
        timeTillDash -= Time.deltaTime;
        if (timeTillDash <= 0)
        {
            chaseSpeed = 8.0f;
            dashTime -= Time.deltaTime;
            if (dashTime <= 0)
            {
                chaseSpeed = 5.0f;
                timeTillDash = 2.0f;
                dashTime = 1.0f;
            }
        }
    }

    private void OnDrawGizmosSelected() // Visualize detection range in editor
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}


