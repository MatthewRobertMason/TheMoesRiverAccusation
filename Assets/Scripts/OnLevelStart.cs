using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLevelStart : MonoBehaviour
{
    public bool isPurgatory = false;
	
	void Start ()
    {        
        AudioManager am = FindObjectOfType<AudioManager>();

        if (isPurgatory)
        {
            am.SwitchMusicToPurgatoryMusic();
        }
        else
        {
            am.SwitchMusicToCultMusic();
        }
	}
}
