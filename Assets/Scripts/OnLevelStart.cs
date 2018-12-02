using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLevelStart : MonoBehaviour
{
    public bool isPurgatory = false;
	
	void Start ()
    {
        if (isPurgatory)
        {
            FindObjectOfType<AudioManager>().SwitchMusicToPurgatoryMusic();
        }
        else
        {
            FindObjectOfType<AudioManager>().SwitchMusicToCultMusic();
        }
	}
}
