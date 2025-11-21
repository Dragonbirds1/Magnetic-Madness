using UnityEngine;

public class AbilitySwap : MonoBehaviour
{
    public static PlayerAbility Player1Object, Player2Object;
    public KeyCode swapKey;
    
    void Update()
    {
        if (Input.GetKeyDown(swapKey))
        {
            SwapAbilities();
        }
    }

    private void SwapAbilities()
    {
        Player1Object.currentForce *= -1;
        Player2Object.currentForce *= -1;
    }
}
