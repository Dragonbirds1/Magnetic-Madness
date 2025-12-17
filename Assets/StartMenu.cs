using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene("MainLevel");
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
        Debug.Log("Henry. I have to quit. Haha.");
    }

    public void OnCreditsButtonPressed()
    {
        SceneManager.LoadScene("Credits");
    }
}