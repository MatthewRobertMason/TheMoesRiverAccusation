using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrapCollider : MonoBehaviour
{
    public AudioClip DeathSound;

    public static HashSet<string> KnownTraps = new HashSet<string>{
        "SpikeBall",
        "Spikes",
        "FirePit",
        "Lava"
    };

	// Use this for initialization
	void Start () {
        Debug.Log("Created trap collider");
        bool found = false;
        foreach(Tilemap map in GameObject.FindObjectsOfType<Tilemap>()) {
            var cell = map.WorldToCell(this.transform.position);
            var tile = map.GetTile(cell);
            if(tile != null) found |= KnownTraps.Contains(tile.name);
        }
        if (!found) {
            Debug.Log("Extra trap collider removed");
            Destroy(this.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
