using System.Threading;
using UnityEngine;

public class ToggleSpark2 : MonoBehaviour
{
    [Header("Spark Negative Animator")]
    [Tooltip("Animator for Negative Spark Animation")]
    public Animator sparkNAnimator;

    [Header("Toggle Key")]
    [Tooltip("Key to toggle Negative Spark Animation")]
    public KeyCode toggleKey;

    [Header("Toggle Duration")]
    [Tooltip("Duration for which the Negative Spark Animation is toggled")]
    public float toggleDuration = 0.0f;


    bool isToggled = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            isToggled = true;
        }
        if (isToggled)
        {
            sparkNAnimator.SetTrigger("SPARKP");
            toggleDuration += Time.deltaTime;
            if (toggleDuration >= 0.267f)
            {
                sparkNAnimator.ResetTrigger("SPARKP");
                toggleDuration = 0.0f;
                isToggled = false;
            }
        }
        
    }
}
