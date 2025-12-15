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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ball"))
        {
            currentCourse.SetActive(false);
            Debug.Log("Ball in the hole!");
            currentHole.SetActive(false);
            newHole.SetActive(true);
            newCourse.SetActive(true);
        }
    }
}
