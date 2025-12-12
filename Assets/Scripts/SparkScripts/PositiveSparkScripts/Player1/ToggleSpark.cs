using System.Threading;
using UnityEngine;

public class ToggleSpark : MonoBehaviour
{
    public Animator sparkPAnimator;
    public KeyCode toggleKey;
    bool isToggled = false;
    public float toggleDuration = 0.0f;

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
            sparkPAnimator.SetTrigger("SPARKP");
            toggleDuration += Time.deltaTime;
            if (toggleDuration >= 0.267f)
            {
                sparkPAnimator.ResetTrigger("SPARKP");
                toggleDuration = 0.0f;
                isToggled = false;
            }
        }
        
    }
}
