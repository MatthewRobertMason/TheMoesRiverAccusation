using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDetonator : MonoBehaviour {

    bool exploding = false;
    public int boomStars = 4;
    public int shootingStars = 3;
    public float flopForce = 10;
    public float shootforce = 100;
    public GameObject starType;
    private AudioSource audioSource;

    public AudioClip fuseSound;
    public AudioClip explodeSound;

    // Use this for initialization
    void Start () {
        audioSource = this.GetComponent<AudioSource>();
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Player") {
            if (!exploding) {
                exploding = true;
                GetComponent<Animator>().enabled = true;
                Invoke("Explode", 4);
                PlaySound(fuseSound);
            }
        }
    }

    private void Remove()
    {
        Destroy(gameObject);
    }

    private void Explode()
    {
        PlaySound(explodeSound);
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
        Invoke("Remove", 5);


        // Create nearby stars
        for(int ii = 0; ii < boomStars; ii++) {
            var star = Instantiate(starType, this.transform.position, Quaternion.identity);
            star.GetComponent<Rigidbody2D>().AddForce(new Vector2(
                Random.Range(-flopForce / Time.fixedDeltaTime, flopForce / Time.fixedDeltaTime), 
                Random.Range(-flopForce / Time.fixedDeltaTime, flopForce / Time.fixedDeltaTime)
            ));
        }

        // Shoot off a couple stars
        for (int ii = 0; ii < shootingStars; ii++) {
            var star = Instantiate(starType, this.transform.position, Quaternion.identity);
            star.GetComponent<Rigidbody2D>().AddForce(new Vector2(
                Random.Range(-shootforce / Time.fixedDeltaTime, shootforce/ Time.fixedDeltaTime),
                Random.Range(-shootforce / Time.fixedDeltaTime, shootforce / Time.fixedDeltaTime)
            ));
        }
    }
}
