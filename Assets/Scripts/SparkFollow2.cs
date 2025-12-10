using UnityEngine;

public class SparkFollow2 : MonoBehaviour
{
    public GameObject player;
    Vector2 offset;
    public GameObject sparkObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        float sparkX = player.transform.position.x;
        float sparkY = player.transform.position.y;
        sparkObject.transform.position = new Vector2(sparkX, sparkY);
    }
}
