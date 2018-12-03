using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelExit : MonoBehaviour {

    public string NextLevel;
    public bool Reset = false;

    public void Exit()
    {
        Debug.LogFormat("Level finished, going to: {0}", NextLevel);
        Scoreboard.FinishLevel();
        if (Reset)
            Scoreboard.Reset();
        UnityEngine.SceneManagement.SceneManager.LoadScene(NextLevel);
    }
}
