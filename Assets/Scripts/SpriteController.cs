﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    private string spriteSheetName;
    public List<string> sheets;

    private int spriteIndex = 0;

    // Use this for initialization
    void Start ()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/Sprites");
        FileInfo[] info = dir.GetFiles("*.png");

        foreach (FileInfo f in info)
        {
            sheets.Add(f.Name.Substring(0, f.Name.Length-4));
        }

        spriteIndex = Random.Range(0, sheets.Count);
        spriteSheetName = sheets[spriteIndex];
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {        
        var subSprites = Resources.LoadAll<Sprite>("Sprites/" + spriteSheetName);
        
        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            Rect spriteRect = ((Sprite)renderer.sprite).rect;
            var newSprite = System.Array.Find(subSprites, item => item.rect == spriteRect);

            if (newSprite)
                renderer.sprite = newSprite;
        }
    }
}
