using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MookMind : MonoBehaviour {

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
            player.PlaySound(player.Rescue);
            Destroy(gameObject);
        }
    }
}
