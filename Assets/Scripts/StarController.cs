using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour {

    float timer = 2;

	// Use this for initialization
	void Start () {
        var torque = Random.Range(-100, 100);
        GetComponent<Rigidbody2D>().AddTorque(torque);
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;
        if(timer <= 0) {
            Destroy(gameObject);
        }

        if (this.GetComponent<Rigidbody2D>().velocity.magnitude < 0.1) {
            Destroy(gameObject);
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player") {
            collision.gameObject.GetComponentInParent<PlayerInput>().Die();
        }
    }
}
