using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class AbilitySwap : MonoBehaviour
{
    public SpriteRenderer Player1Sprite, Player2Sprite;
    public GameObject Player1Object, Player2Object;
    private KeyCode key;
    private KeyCode pullKey;
    private KeyCode pushKey;
    public Color32 colorA = new Color32(255, 153, 0, 255);
    public Color32 colorB = new Color32(238, 255, 255, 255);
    private bool isAbilitySwapped = false;
    PlayerRadius playerRadius;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRadius = GetComponent<PlayerRadius>();
        key = KeyCode.Q; // Default key for swapping abilities
        pullKey = KeyCode.E; // Default key for pulling ability
        pushKey = KeyCode.R; // Default key for pushing ability

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key))
        {
            isAbilitySwapped = !isAbilitySwapped;
            SwapAbilities(isAbilitySwapped);
        }
        if (Input.GetKeyDown(pullKey))
        {
            Debug.Log("Pull Ability Activated");
            if (Player1Object != null && Player2Object != null)
            {
                Vector2 direction = Player1Object.transform.position - Player2Object.transform.position;
                direction.Normalize();
                Player2Object.transform.position = direction * Time.deltaTime;
                
            }
        }
       
        if (Input.GetKeyDown(pushKey))
        {
            Debug.Log("Push Ability Activated");
            // Implement push ability logic here
        }
    }
    private void SwapAbilities(bool swap)
    {
        if (swap)
        {
            Debug.Log("Abilities Swapped: Now using Ability Set B");
            Player2Sprite.color = colorA;
            Player1Sprite.color = colorB;
            pullKey = KeyCode.R; // Change pull ability key
            pushKey = KeyCode.E; // Change push ability key
        }
        else
        {
            Debug.Log("Abilities Swapped: Now using Ability Set A");
            Player2Sprite.color = colorB;
            Player1Sprite.color = colorA;
            pullKey = KeyCode.E; // Revert pull ability key
            pushKey = KeyCode.R; // Revert push ability key
        }
    }
}
