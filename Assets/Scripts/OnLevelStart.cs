using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnLevelStart : MonoBehaviour
{
    public bool isPurgatory = false;
    public bool isGoodEnding = false;

    void Start()
    {
        /*
        if (isPurgatory) {
            foreach (Scoreboard score in FindObjectsOfType<Scoreboard>()) {
                score.Lives = 2;
            }
        }
        */

        if (isGoodEnding == true)
        {
            Scoreboard.gameBeaten = true;
        }

        if (Scoreboard.gameBeaten)
        {
            PlayerInput pi = FindObjectOfType<PlayerInput>();
            pi.VisceraLifespan = 120.0f;
            pi.GenerateVisceraOnDeathAmount = 30;
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
