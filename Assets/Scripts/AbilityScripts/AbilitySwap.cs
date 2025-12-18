using UnityEngine;

public class AbilitySwap : MonoBehaviour
{
    public static PlayerAbility Player1Object, Player2Object;
    public KeyCode swapKey, swapKey2;
    public WackAMole wackAMole;
    public Player1Death player1Death;
    public Player2Death player2Death;
    public bool death = false;
    public bool isPull = true;

    void Update()
    {
        if (wackAMole.freezeGameplay == false || death == false)
        {
            if (Input.GetKeyDown(swapKey) || Input.GetKeyDown(swapKey2))
            {
                SwapAbilities();
            }
        }
        else if (wackAMole.freezeGameplay == true || death == true)
        {
            return;
        }
        if (Player1Object.currentForce > 0)
        {
            isPull = true;
        }
        else if (Player1Object.currentForce < 0)
        {
            isPull = false;
        }
    }

    private void SwapAbilities()
    {
        Player1Object.currentForce *= -1;
        Player2Object.currentForce *= -1;
    }
}
