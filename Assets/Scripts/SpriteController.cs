using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpriteController : MonoBehaviour
{
    private string spriteSheetName;
    public List<string> sheets;

    private int spriteIndex = 0;

    // Use this for initialization
    void Start ()
    {
        //DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Resources/Sprites");
        //FileInfo[] info = dir.GetFiles("*.png");


        Texture2D[] objs = Resources.LoadAll<Texture2D>("Sprites/");
        foreach (Texture2D tex in objs)
        {
            //Debug.Log("Name: " + tex.name);
            sheets.Add(tex.name);
        }

        //foreach (FileInfo f in info)
        //{
        //    sheets.Add(f.Name.Substring(0, f.Name.Length-4));
        //}

        spriteIndex = Random.Range(0, sheets.Count);
        spriteSheetName = sheets[spriteIndex];

        LoadSprite();
    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        LoadSprite();
    }

    void LoadSprite()
    {
        var subSprites = Resources.LoadAll<Sprite>("Sprites/" + spriteSheetName);

        foreach (var renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            Rect spriteRect = ((Sprite)renderer.sprite).rect;
            var newSprite = System.Array.Find(subSprites, item => item.rect == spriteRect);

            if (newSprite)
            {
                renderer.sprite = newSprite;

                if (newSprite.rect == new Rect(0.0f, 72.0f, 16.0f, 24.0f))
                {
                    if (GameObject.Find("LivesImage") != null)
                    {
                        Image lives = GameObject.Find("LivesImage").GetComponent<Image>();
                        if (lives != null)
                            lives.sprite = newSprite;
                    }
                }
            }
        }
    }
}
