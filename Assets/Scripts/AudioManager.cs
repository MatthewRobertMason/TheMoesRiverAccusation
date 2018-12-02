using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{    
    static private GameObject self;
    private void Awake()
    {
        if (self == null)
        {
            self = gameObject;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public AudioClip purgatoryMusic;
    public AudioClip cultMusic;
    private AudioSource audioSource;

    // Use this for initialization
    void Start ()
    {
        audioSource = self.GetComponent<AudioSource>();

        audioSource.clip = purgatoryMusic;
        audioSource.loop = true;
        audioSource.Play();
    }
	
    public void SwitchMusicToPurgatoryMusic()
    {
        audioSource.Stop();
        float time = 0.0f;
        time = audioSource.time;
        audioSource.clip = purgatoryMusic;
        audioSource.time = time;
        audioSource.Play();
    }

    public void SwitchMusicToCultMusic()
    {
        audioSource.Stop();
        float time = 0.0f;
        time = audioSource.time;
        audioSource.clip = cultMusic;
        audioSource.time = time;
        audioSource.Play();
    }
}
