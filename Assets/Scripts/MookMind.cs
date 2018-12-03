using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MookMind : MonoBehaviour {

    private AudioSource audioSource;
    public AudioClip Rescue;

    // Use this for initialization
    void Start () {
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();
    }

    void Remove()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var player = collision.gameObject.GetComponent<PlayerInput>();
        if (player != null) {
            Scoreboard.SaveMook();
            PlaySound(Rescue);
            foreach(SpriteRenderer sr in GetComponentsInChildren<SpriteRenderer>()) sr.enabled = false;
            foreach(Collider2D col in GetComponentsInChildren<Collider2D>()) col.enabled = false;
            Invoke("Remove", 2);
        }
    }
}
