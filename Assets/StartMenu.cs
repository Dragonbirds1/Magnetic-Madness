using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
  public void OnStartButtonPressed()
    {
    SceneManager.LoadScene("MainLevel");
    }
}
