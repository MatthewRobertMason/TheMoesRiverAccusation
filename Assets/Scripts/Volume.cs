using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    public Scrollbar volume;

    
	// Use this for initialization
	void Start ()
    {
        // Match the volume to the audioManager
        volume.value = FindObjectOfType<AudioManager>().GetComponent<AudioSource>().volume;

        // Make sure everything else has at least started
        //Invoke("setAudio", 0.1f);
        setAudio();
	}

    public void setAudio()
    {
        AudioSource[] aSource = FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in aSource)
        {
            source.volume = volume.value;
        }
    }
}