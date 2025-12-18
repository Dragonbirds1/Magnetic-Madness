using UnityEngine;

public class Lever : MonoBehaviour
{
    [Header("PlayerAbility Script")]
    [Tooltip("Reference to the PlayerAbility script")]
    public PlayerAbility playerAbility;

    [Header("Lever GameObject")]
    [Tooltip("The Lever GameObject")]
    public GameObject lever;

    [Header("Lever State")]
    [Tooltip("Is the lever currently on?")]
    public bool isLeverOn;
    [Tooltip("Is the lever currently off?")]
    public bool isLeverOff;

    [Header("Sprite Renderers")]
    [Tooltip("Sprite Renderer for lever off")]
    public Sprite leverOffSR;
    [Tooltip("Sprite Renderer for lever on")]
    public Sprite leverOnSR;

    [Header("Animator for Lever")]
    [Tooltip("Animator component for the lever")]
    public Animator leverAnimator;

    bool isOn = false;
    bool isOff = false;

    private SpriteRenderer m_SpriteRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLeverOn)
        {
            m_SpriteRenderer.sprite = leverOnSR;
            isLeverOn = false;
            leverOn();
        }
        else if (isLeverOff)
        {
            m_SpriteRenderer.sprite = leverOffSR;
            isLeverOff = false;
            leverOff();
        }
    }

    void leverOn()
    {
        Debug.Log("Lever is On - Door Open, GOOD LUCK >:)");
        leverAnimator.SetBool("Open", true);
        leverAnimator.SetBool("Close", false);
    }
    void leverOff()
    {
        Debug.Log("Lever is Off - Door Closed, YOU DIRTY CHICKEN >:(");
        leverAnimator.SetBool("Close", true);
        leverAnimator.SetBool("Open", false);
    }
}
