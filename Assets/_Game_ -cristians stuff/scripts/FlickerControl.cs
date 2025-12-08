using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Rendering.Universal;
public class FlickerControl : MonoBehaviour
{
    public bool isFlickering = false;
    public float timeDelay;

    

    void Update()
    {
      if (isFlickering == false)
        {
            StartCoroutine(FlickeringLight());
        }
        

    }



    IEnumerator FlickeringLight()
    {
        isFlickering = true;
        this.gameObject.GetComponent<Light2D>().enabled = false;
        timeDelay = Random.Range(0.2f, 2.0f);
        yield return new WaitForSeconds(timeDelay);
        this.gameObject.GetComponent<Light2D>().enabled = true;
        timeDelay = Random.Range(0.2f, 3.0f);
        yield return new WaitForSeconds(timeDelay);
        isFlickering = false;
    }


}
