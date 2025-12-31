using UnityEngine;

public class Hole : MonoBehaviour
{
    [Header("Ball GameObject")]
    [Tooltip("Assign the ball GameObject here to detect when it enters the hole.")]
    public GameObject ball;

    [Header("Current Hole GameObject")]
    [Tooltip("Assign the current hole GameObject here.")]
    public GameObject currentHole;

    [Header("Current Course GameObject")]
    [Tooltip("Assign the current course GameObject here.")]
    public GameObject currentCourse;

    [Header("New Hole GameObject")]
    [Tooltip("Assign the new hole GameObject here to transition to the next hole.")]
    public GameObject newHole;

    [Header("New Course GameObject")]
    [Tooltip("Assign the new course GameObject here to transition to the next course.")]
    public GameObject newCourse;

    [Header("Ball Tp")]
    [Tooltip("Transform golf ball to new spawn")]
    public GameObject newSpawn;

    public void Update()
    {
        // Get golf ball position
        float ballPos = Vector2.Distance(transform.position, ball.transform.position);

        // Get spawn position
        float spawnPos = Vector2.Distance(transform.position, newSpawn.transform.position);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            Debug.Log("Ball in the hole!");
            // Tp golf ball to new spawn
            //ball.transform.position = newSpawn.transform.position;
        }
    }
}
