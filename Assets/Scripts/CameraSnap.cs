using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSnap : MonoBehaviour
{
    public int pixelsPerUnit = 16;
    private Transform rootNode;

	// Use this for initialization
	void Start ()
    {
        rootNode = transform.parent;
	}
	
	// Update is called once per frame
	void Update ()
    {
        this.transform.position = new Vector3(
            Mathf.Floor(rootNode.position.x * 16.0f) / 16.0f,
            Mathf.Floor(rootNode.position.y * 16.0f) / 16.0f,
            -10.0f
        );
	}
}
