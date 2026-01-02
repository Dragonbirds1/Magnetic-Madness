using UnityEngine;
using UnityEngine.InputSystem;

public class BowlingInputRefs : MonoBehaviour
{
    [Header("PlayerInput (THIS ties inputs to Player2)")]
    public PlayerInput playerInput;   // drag Player2's PlayerInput here

    [Header("Action names (must match Input Actions asset)")]
    public string aimActionName = "Aim";       // Value(Vector2)
    public string chargeActionName = "Charge"; // Button or Axis
    public string throwActionName = "Throw";   // Button

    public InputAction Aim => playerInput ? playerInput.actions[aimActionName] : null;
    public InputAction Charge => playerInput ? playerInput.actions[chargeActionName] : null;
    public InputAction Throw => playerInput ? playerInput.actions[throwActionName] : null;
}
