using UnityEngine;

public class MoleHit : MonoBehaviour
{
    public GameObject mole;
    public Hammer hammer;
    //public GameObject
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        hammer = GetComponent<Hammer>();
    }

    // Update is called once per frame
    public void Update()
    {
    }
    public void GetHit()
    {
        
    }
}
