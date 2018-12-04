using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MookMind : MonoBehaviour {

    public GameObject SoundObject;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerInput>();
        if (player != null) {
            Scoreboard.SaveMook();
            var sound = Instantiate(SoundObject);
            sound.transform.position = this.transform.position;
            Destroy(gameObject);
        }
    }
}
