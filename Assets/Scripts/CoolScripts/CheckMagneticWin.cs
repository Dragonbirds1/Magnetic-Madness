using UnityEngine;

public class CheckMagneticWin : MonoBehaviour
{
    [Header("Player2Win Script")]
    public Player2Win player2Win;

    [Header("Player1Win")]
    public Player1Win player1Win;

    [Header("Player1 and Player2 References")]
    public GameObject player1, player2;

    [Header("Win Logic Variables/References")]
    // Add any additional variables or references needed for win logic here
    public GameObject WIN;
    public AudioSource winSong;
    public AudioSource mainMusic;
    public GameObject winMusicObject;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        winMusicObject.SetActive(false);
        WIN.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (player1.activeSelf == false && player2.activeSelf == false)
        {
            Win();
        }
    }

    void Win()
    {
        winMusicObject.SetActive(true);
        player1Win.wait.SetActive(false);
        player2Win.wait.SetActive(false);

        // Insert win logic here
        WIN.SetActive(true);
        mainMusic.Stop();
    }
}
