using UnityEngine;

public class CardReader2 : MonoBehaviour
{
    [Header("TriggerStuff Script")]
    [Tooltip("Reference to the TriggerStuff script")]
    public TriggerStuff triggerStuff;

    [Header("CardGate1 Script")]
    [Tooltip("Reference to the CardGate1 script")]
    public CardGate1 cardGate1;

    [Header("CardReader GameObject")]
    [Tooltip("The CardReader GameObject")]
    public GameObject cardReader;

    [Header("CardReader Settings")]
    [Tooltip("Radius for cardReader")]
    public float radToCardReader;

    [Header("CardReader State")]
    [Tooltip("Is the CardReader active?")]
    public bool isCardReaderActive;

    [Header("Player GameObject")]
    [Tooltip("The Player GameObject")]
    public GameObject player;

    [Header("Player2 GameObject")]
    [Tooltip("The Player2 GameObject")]
    public GameObject player2;

    [Header("Sprite Renderers")]
    [Tooltip("Sprite Renderer for incorrect")]
    public Sprite incorrectSR;
    [Tooltip("Sprite Renderer for correct")]
    public Sprite correctSR;
    [Tooltip("Sprite Renderer for used")]
    public Sprite usedSR;
    [Tooltip("Sprite Renderer for card reader")]
    public Sprite cardReaderSR;

    [Header("Timers")]
    [Tooltip("Timer for correct usage")]
    public float correctTimer;
    [Tooltip("Timer for incorrect usage")]
    public float incorrectTimer;

    [Header("KeyCode")]
    [Tooltip("Key to use the Card Reader")]
    public KeyCode useCardReaderKey;

    bool isCorrect;
    bool isIncorrect;
    private SpriteRenderer m_SpriteRenderer;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float playerPos = Vector2.Distance(transform.position, player.transform.position);
        float player2Pos = Vector2.Distance(transform.position, player2.transform.position);
        if (isCorrect)
        {
            correctTimer += Time.deltaTime;
            if (correctTimer >= 1.0f)
            {
                m_SpriteRenderer.sprite = usedSR;
                isCorrect = false;
                correctTimer = 0.0f;
            }
        }
        if (isIncorrect)
        {
            incorrectTimer += Time.deltaTime;
            if (incorrectTimer >= 1.0f)
            {
                m_SpriteRenderer.sprite = cardReaderSR;
                isIncorrect = false;
                incorrectTimer = 0.0f;
            }
        }
        if (isCardReaderActive)
        {
            if (playerPos <= radToCardReader || player2Pos <= radToCardReader)
            {
                if (Input.GetKeyDown(useCardReaderKey))
                {
                    if (triggerStuff.keycard2.activeSelf == false && triggerStuff.canClaimCard2 == true)
                    {
                        isCardReaderActive = false;
                        isCorrect = true;
                        m_SpriteRenderer.sprite = correctSR;
                        cardGate1.currentActiveCardReaderCount += 1;
                        Debug.Log("Card Reader Used, Door Unlocked!");
                    }
                    else
                    {
                        isIncorrect = true;
                        m_SpriteRenderer.sprite = incorrectSR;
                        Debug.Log("You need a Keycard to use the Card Reader.");
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, radToCardReader);
    }
}
