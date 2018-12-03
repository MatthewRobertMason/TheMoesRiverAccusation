﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    public Scrollbar volumeBar;
    public float saveVolume = 0.0f;
    
	// Use this for initialization
	void Start ()
    {
        // Match the volume to the audioManager
        saveVolume = FindObjectOfType<AudioManager>().GetComponent<AudioSource>().volume;
        volumeBar.value = saveVolume;

        // Make sure everything else has at least started
        //Invoke("setAudio", 0.1f);
        //SetAudio();
	}

    public void SetAudio()
    {
        saveVolume = volumeBar.value;
    }

    public void Update()
    {
        AudioSource[] aSource = FindObjectsOfType<AudioSource>();

        foreach (AudioSource source in aSource)
        {
            source.volume = saveVolume;
        }
    }
}