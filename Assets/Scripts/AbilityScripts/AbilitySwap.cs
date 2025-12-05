using UnityEngine;

public class AbilitySwap : MonoBehaviour
{
    public static PlayerAbility Player1Object, Player2Object;
    public KeyCode swapKey, swapKey2;
    public WackAMole wackAMole;
    
    void Update()
    {
        if (wackAMole.freezeGameplay == false)
        {
            if (Input.GetKeyDown(swapKey) || Input.GetKeyDown(swapKey2))
            {
                SwapAbilities();
            }
        }
        else if (wackAMole.freezeGameplay == true)
        {
            return;
        }
    }

    private void SwapAbilities()
    {
        Player1Object.currentForce *= -1;
        Player2Object.currentForce *= -1;
    }
}
