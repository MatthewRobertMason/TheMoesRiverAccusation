﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreboard : MonoBehaviour {

    static private GameObject self = null;

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
        if (GetScoreboard().Lives > 0) GetScoreboard().Lives--;
        Debug.LogFormat("Life lost, {0} remaining.", GetScoreboard().Lives);
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

    public int Lives = 10;
    
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
