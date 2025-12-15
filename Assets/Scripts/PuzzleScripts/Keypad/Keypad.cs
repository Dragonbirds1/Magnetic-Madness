using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections;

public class Keypad : MonoBehaviour
{
    [Header("Keypad Settings")]
    public string correctCode = "123456";
    public TMP_Text displayText;
    private string inputCode = "";
    private bool isUnlocked = false;

    [Header("Game Objects")]
    public GameObject doorObject;

    [Header("Sounds")]
    public AudioSource audioSource;
    public AudioClip keySound;
    public AudioClip correctSound;
    public AudioClip incorrectSound;
    public AudioClip incompleteSound;
    public AudioClip clearSound;

    [Header("Buttons")]
    public Button[] digitButtons;      // 0–9 buttons
    public Button enterButton;
    public Button clearButton;

    [Header("Highlight")]
    public RectTransform highlight;    // Outline image around selected button
    public float highlightPadding = 10f;

    private int selectedIndex = 0;
    private bool keypadActive = false;
    private Button[] allButtons;

    private bool inputLocked = false; // locks input when code is incorrect or incomplete

    public GameObject keypadPanel;

    public float clearTime = 0.0f;
    private bool startClear = false;

    void Start()
    {
        doorObject.SetActive(true);

        // Combine all buttons into one array
        allButtons = new Button[digitButtons.Length + 2];
        digitButtons.CopyTo(allButtons, 0);
        allButtons[digitButtons.Length] = enterButton;
        allButtons[digitButtons.Length + 1] = clearButton;

        // Connect button click events
        for (int i = 0; i < digitButtons.Length; i++)
        {
            int index = i;
            digitButtons[i].onClick.AddListener(() => PressDigit(index));
            AddHoverHighlight(digitButtons[i], index);
        }

        enterButton.onClick.AddListener(PressEnter);
        clearButton.onClick.AddListener(PressClear);

        AddHoverHighlight(enterButton, digitButtons.Length);
        AddHoverHighlight(clearButton, digitButtons.Length + 1);
    }

    void Update()
    {
        displayText.text = inputCode;

        if (startClear)
        {
            clearTime += Time.deltaTime;
            if (clearTime >= 2.0f)
            {
                keypadPanel.SetActive(false);
                inputCode = "";
                clearTime = 0.0f;
                startClear = false;
                inputLocked = false;
                UnlockAllButtons();
            }
        }

        if (!keypadActive) return;

        HandleNavigation();
    }

    // --------------------------
    // BUTTON HANDLING
    // --------------------------

    public void PressDigit(int number)
    {
        if (inputLocked) return;  // Ignore clicks when locked
        if (inputCode.Length >= correctCode.Length) return;

        audioSource.PlayOneShot(keySound);
        inputCode += number.ToString();
    }

    public void PressClear()
    {
        if (inputLocked) return;

        audioSource.PlayOneShot(clearSound);
        inputCode = "";
    }

    public void PressEnter()
    {
        if (inputLocked) return;

        if (inputCode.Length == correctCode.Length)
        {
            if (inputCode == correctCode)
            {
                inputCode = "CORRECT";
                audioSource.PlayOneShot(correctSound);
                isUnlocked = true;
                LockAllButtons();
                startClear = true;
                doorObject.SetActive(false);
            }
            else
            {
                inputCode = "INCORRECT";
                audioSource.PlayOneShot(incorrectSound);
                LockAllButtons();
                StartCoroutine(ResetAfterDelay());
            }
        }
        else
        {
            inputCode = "INCOMPLETE";
            audioSource.PlayOneShot(incompleteSound);
            LockAllButtons();
            StartCoroutine(ResetAfterDelay());
        }
    }

    // --------------------------
    // LOCK / UNLOCK BUTTONS
    // --------------------------

    private void LockAllButtons()
    {
        inputLocked = true;
        foreach (Button btn in allButtons)
        {
            btn.interactable = false;
        }
    }

    private void UnlockAllButtons()
    {
        inputLocked = false;
        foreach (Button btn in allButtons)
        {
            btn.interactable = true;
        }
    }

    // --------------------------
    // NAVIGATION / CONTROLLER
    // --------------------------

    private void HandleNavigation()
    {
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            SelectButton(0);
            return;
        }

        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;

        for (int i = 0; i < allButtons.Length; i++)
        {
            if (allButtons[i].gameObject == selectedObj)
            {
                selectedIndex = i;
                MoveHighlightTo(allButtons[i]);
                break;
            }
        }
    }

    private void SelectButton(int index)
    {
        selectedIndex = index;
        EventSystem.current.SetSelectedGameObject(allButtons[index].gameObject);
        MoveHighlightTo(allButtons[index]);
    }

    // --------------------------
    // HIGHLIGHT HANDLING
    // --------------------------

    private void MoveHighlightTo(Button button)
    {
        RectTransform target = button.GetComponent<RectTransform>();
        highlight.gameObject.SetActive(true);
        highlight.position = target.position;
        highlight.sizeDelta = target.sizeDelta + new Vector2(highlightPadding, highlightPadding);
    }

    private void AddHoverHighlight(Button button, int index)
    {
        EventTrigger trigger = button.gameObject.AddComponent<EventTrigger>();

        var entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((data) =>
        {
            SelectButton(index);
        });

        trigger.triggers.Add(entry);
    }

    // --------------------------
    // DELAY RESET
    // --------------------------

    private IEnumerator ResetAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        inputCode = "";
        UnlockAllButtons();
    }

    public void OpenKeypad()
    {
        keypadActive = true;
        SelectButton(0);
    }

    public void CloseKeypad()
    {
        keypadActive = false;
        highlight.gameObject.SetActive(false);
    }
}
