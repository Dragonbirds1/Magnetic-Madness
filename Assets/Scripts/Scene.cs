using UnityEngine;
using UnityEngine.SceneManagement;

public class Scene : MonoBehaviour
{
    public GameObject Credits;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MAINMAINLEVEL");
    }

    public void Credit()
    {
        Credits.SetActive(true);
    }

    public void Leave()
    {
        Application.Quit();
    }
}
