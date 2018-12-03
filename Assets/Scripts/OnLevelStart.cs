using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLevelStart : MonoBehaviour
{
    public bool isPurgatory = false;

    void Start()
    {
        if (isPurgatory) {
            foreach (Scoreboard score in FindObjectsOfType<Scoreboard>()) {
                score.Lives = 2;
            }
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
