using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{    
    static private GameObject self;

    public AudioClip purgatoryMusic;
    public AudioClip cultMusic;
    private AudioSource audioSource;

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

        audioSource = self.GetComponent<AudioSource>();
        audioSource.clip = purgatoryMusic;
        audioSource.loop = true;
        audioSource.Play();
    }
    
    // Use this for initialization
    void Start ()
    {
        
    }
	
    public void SwitchMusicToPurgatoryMusic()
    {
        audioSource = self.GetComponent<AudioSource>();
        float time = 0.0f;
        time = audioSource.time;
        audioSource.Stop();
        audioSource.clip = purgatoryMusic;
        audioSource.time = time;
        audioSource.Play();
    }

    public void SwitchMusicToCultMusic()
    {
        audioSource = self.GetComponent<AudioSource>();
        float time = 0.0f;
        time = audioSource.time;
        audioSource.Stop();
        audioSource.clip = cultMusic;
        audioSource.time = time;
        audioSource.Play();
    }
}
