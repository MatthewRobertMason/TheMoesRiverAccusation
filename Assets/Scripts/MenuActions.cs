using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuActions : MonoBehaviour {

	public void StartGame()
    {
        Debug.Log("Starting Game...");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Map001");
    }
}
