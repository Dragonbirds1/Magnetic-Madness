using UnityEngine;
using UnityEngine.InputSystem; // Required for the new Input System
using UnityEngine.EventSystems; // For UI interaction

/// <summary>
/// Moves a cursor using a gamepad or keyboard and allows UI interaction.
/// Attach this to a GameObject with a RectTransform (e.g., an Image) inside a Canvas.
/// </summary>
public class ControllerCursor : MonoBehaviour
{
    [Header("Cursor Settings")]
    public float moveSpeed = 800f; // Pixels per second
    public RectTransform canvasRect; // Reference to the parent Canvas RectTransform

    [Header("Input Actions")]
    public InputAction moveAction; // Vector2 input for movement
    public InputAction clickAction; // Button input for clicking

    public RectTransform cursorRect;
    private Camera uiCamera;

    private void OnEnable()
    {
        moveAction.Enable();
        clickAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        clickAction.Disable();
    }

    private void Start()
    {
        cursorRect = GetComponent<RectTransform>();

        // Get the camera rendering the UI
        Canvas canvas = canvasRect.GetComponent<Canvas>();
        uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        // Start cursor in center
        cursorRect.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        MoveCursor();
        HandleClick();
    }

    private void MoveCursor()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // Move cursor based on input
        Vector2 newPos = cursorRect.anchoredPosition + moveInput * moveSpeed * Time.deltaTime;

        // Clamp position inside canvas
        Vector2 canvasSize = canvasRect.sizeDelta;
        newPos.x = Mathf.Clamp(newPos.x, -canvasSize.x / 2, canvasSize.x / 2);
        newPos.y = Mathf.Clamp(newPos.y, -canvasSize.y / 2, canvasSize.y / 2);

        cursorRect.anchoredPosition = newPos;
    }

    private void HandleClick()
    {
        if (clickAction.WasPressedThisFrame())
        {
            // Simulate a UI click at the cursor position
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = RectTransformUtility.WorldToScreenPoint(uiCamera, cursorRect.position)
            };

            var results = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);

            foreach (var result in results)
            {
                ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerClickHandler);
            }
        }
    }
}
