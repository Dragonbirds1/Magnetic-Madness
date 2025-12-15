using UnityEngine;

[System.Serializable]
public class CustomInput
{
    public string name;                 // Descriptive name
    public KeyCode keyboardPositive;    // Keyboard positive key
    public KeyCode keyboardNegative;    // Keyboard negative key
    public KeyCode joystickPositive;    // Joystick positive button
    public KeyCode joystickNegative;    // Joystick negative button

    // Returns -1, 0, or 1 for digital inputs
    public int GetValue()
    {
        int value = 0;

        if (Input.GetKey(keyboardPositive) || Input.GetKey(joystickPositive))
            value += 1;

        if (Input.GetKey(keyboardNegative) || Input.GetKey(joystickNegative))
            value -= 1;

        return Mathf.Clamp(value, -1, 1);
    }

    // Returns a "simulated" analog float between -1 and 1
    public float GetAnalogValue()
    {
        float value = 0f;

        if (Input.GetKey(keyboardPositive) || Input.GetKey(joystickPositive))
            value += 1f;

        if (Input.GetKey(keyboardNegative) || Input.GetKey(joystickNegative))
            value -= 1f;

        return Mathf.Clamp(value, -1f, 1f);
    }
}
