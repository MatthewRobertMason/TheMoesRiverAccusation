using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayRescueSound : MonoBehaviour {
    private AudioSource audioSource;
    public AudioClip Rescue;

    // Use this for initialization
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlaySound(Rescue);
        Invoke("Remove", 5);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Remove()
    {
        Destroy(gameObject);
    }

    private void PlaySound(AudioClip clip)
    {
        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = false;
        audioSource.Play();
    }
}
