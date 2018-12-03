using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLevelStart : MonoBehaviour
{
    public bool isPurgatory = false;

    void Start()
    {
        Scoreboard score = FindObjectOfType<Scoreboard>();
        if (score != null)
        {
            score.Lives = 2;
        }

        AudioManager am = FindObjectOfType<AudioManager>();
        if (am != null)
        {
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
}
