using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

    static private GameObject self = null;
    static public bool gameBeaten = false;

    static public Scoreboard GetScoreboard()
    {
        return self.GetComponent<Scoreboard>();
    }

    static public int GetLives()
    {
        return GetScoreboard().Lives;
    }

    static public void LostLife()
    {
        if (GetScoreboard().Lives > 0) {
            GetScoreboard().Deaths++;
            GetScoreboard().Lives--;
        }

        Debug.LogFormat("Life lost, {0} remaining.", GetScoreboard().Lives);
    }

    static public void SaveMook()
    {
        GetScoreboard().Lives++;
        GetScoreboard().DudeSaves++;
    }

    static public void FinishLevel()
    {
        GetScoreboard().LevelsFinished++;
    }

    private void Awake()
    {
        if(self == null) {
            self = gameObject;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    static public void Reset()
    {
        GetScoreboard().Lives = 10;
        GetScoreboard().Deaths = 0;
        GetScoreboard().LevelsFinished = 0;
        GetScoreboard().DudeSaves = 0;
    }


    public int Lives = 10;
    public int Deaths = 0;
    public int LevelsFinished = 0;
    public int DudeSaves = 0;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        GameObject textObj = GameObject.Find("LivesText");
        if(textObj != null) {
            UnityEngine.UI.Text text = textObj.GetComponent<UnityEngine.UI.Text>();
            text.text = Lives.ToString();
        }
	}
}
