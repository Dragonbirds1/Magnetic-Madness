using UnityEngine;

public class OpenAndCloseMap : MonoBehaviour
{
    /// <summary>
    /// This script is responsible for opening and closing the in-game map.
    /// The in-game map allows players to navigate and explore the game world more effectively.
    /// More functionality to be added in future updates.
    /// </summary>
    [Header("Map Reference")]
    public GameObject inGameMap;

    [Header("KeyCodes")]
    public KeyCode toggleMapKey;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inGameMap.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(toggleMapKey))
        {
            // Toggle the map's active state
            inGameMap.SetActive(true);
        }
        else if (Input.GetKeyUp(toggleMapKey))
        {
            // Toggle the map's disactive state
            inGameMap.SetActive(false);
        }
    }
}
