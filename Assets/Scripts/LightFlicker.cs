using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    public float variationInScale = 0.1f;

    private float xScale = 0.0f;
    private float yScale = 0.0f;

    // Use this for initialization
    void Start ()
    {
        xScale = this.transform.localScale.x;
        yScale = this.transform.localScale.y;   
    }
	
	// Update is called once per frame
	void Update ()
    {
        float changeAmount = (variationInScale / 10.0f);
        float change = Random.Range(-changeAmount, changeAmount);

        float x = this.transform.localScale.x + change;
        float y = this.transform.localScale.y + change;
        
        x = Mathf.Clamp(x, xScale - variationInScale, xScale + variationInScale);
        y = Mathf.Clamp(y, yScale - variationInScale, yScale + variationInScale);

        this.transform.localScale = new Vector3(x, y, 1.0f);
    }
}
