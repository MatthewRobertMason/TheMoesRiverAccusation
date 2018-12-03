using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageArea : MonoBehaviour {

    public GameObject displayArea;
    public GameObject textArea;
    public string[] Messages;

    private void Start()
    {
        displayArea.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player") {
            Debug.Log("Entered Message area");
            displayArea.SetActive(true);
            textArea.GetComponent<UnityEngine.UI.Text>().text = Messages[Random.Range(0, Messages.Length)];
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player") {
            Debug.Log("Exit Message area");
            displayArea.SetActive(false);
        }
    }

}
