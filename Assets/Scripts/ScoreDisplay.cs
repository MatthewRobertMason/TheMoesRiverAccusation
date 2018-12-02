using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour {

    public GameObject LivesText;
    public GameObject AltersText;

    // Use this for initialization
    void Start () {
        Scoreboard board = Scoreboard.GetScoreboard();
        LivesText.GetComponent<Text>().text = board.Deaths.ToString();
        AltersText.GetComponent<Text>().text = board.LevelsFinished.ToString();
    }
}
