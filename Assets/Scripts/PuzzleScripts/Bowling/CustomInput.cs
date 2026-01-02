using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class CustomInput
{
    [Header("Input System Actions")]
    [Tooltip("Value action (float). Use for aim/hook/steer. Expected control: 1D Axis or Vector2.")]
    public InputActionReference valueAction;

    [Tooltip("Button action. Use for throw / interact. Expected control: Button.")]
    public InputActionReference buttonAction;

    [Header("Value Settings")]
    [Tooltip("If your valueAction is a Vector2, which axis should be used?")]
    public Axis2D vector2Axis = Axis2D.X;

    [Tooltip("Deadzone for analog sticks / triggers.")]
    [Range(0f, 0.5f)] public float deadzone = 0.15f;

    public enum Axis2D { X, Y }

    // ---------- LIFECYCLE ----------
    public void Enable()
    {
        if (valueAction && valueAction.action != null) valueAction.action.Enable();
        if (buttonAction && buttonAction.action != null) buttonAction.action.Enable();
    }

    public void Disable()
    {
        if (valueAction && valueAction.action != null) valueAction.action.Disable();
        if (buttonAction && buttonAction.action != null) buttonAction.action.Disable();
    }

    // ---------- VALUE ----------
    /// <summary>Returns a float in [-1,1] from a 1D Axis OR from a Vector2 axis.</summary>
    public float ReadValue()
    {
        if (valueAction == null || valueAction.action == null) return 0f;

        var act = valueAction.action;

        // If action is bound to Vector2 (stick), read Vector2 and select axis
        // If bound to float (1D axis), ReadValue<Vector2>() will throw, so use try/catch.
        try
        {
            Vector2 v = act.ReadValue<Vector2>();
            float val = (vector2Axis == Axis2D.X) ? v.x : v.y;
            return ApplyDeadzone(val);
        }
        catch
        {
            float val = act.ReadValue<float>();
            return ApplyDeadzone(val);
        }
    }

    /// <summary>Digital version of ReadValue(): -1, 0, 1</summary>
    public int GetDigital()
    {
        float v = ReadValue();
        if (v > deadzone) return 1;
        if (v < -deadzone) return -1;
        return 0;
    }

    // ---------- BUTTON ----------
    public bool PressedThisFrame()
    {
        return buttonAction != null && buttonAction.action != null && buttonAction.action.WasPressedThisFrame();
    }

    public bool ReleasedThisFrame()
    {
        return buttonAction != null && buttonAction.action != null && buttonAction.action.WasReleasedThisFrame();
    }

    public bool IsHeld()
    {
        return buttonAction != null && buttonAction.action != null && buttonAction.action.IsPressed();
    }

    // ---------- HELPERS ----------
    float ApplyDeadzone(float v)
    {
        return Mathf.Abs(v) < deadzone ? 0f : Mathf.Clamp(v, -1f, 1f);
    }
}
