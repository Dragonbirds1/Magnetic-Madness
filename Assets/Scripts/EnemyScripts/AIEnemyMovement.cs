using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemyMovement : MonoBehaviour
{
    public float speed = 3.0f;
    public float waitTime = 1;
    public List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    private Rigidbody2D rb;
    private Vector3 currentPosition;
    private Vector3 currentVelocity;
    private Vector3 currentRotation;
    private Vector3 currentRotationDirection;
    private Vector3 currentVelocityDirection;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        transform.position = waypoints[0].position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
        {
            waitTime -= Time.deltaTime;
            if (waitTime <= 0)
            {
                currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Count;
                waitTime = 3;
            }
        } 
    }
}
