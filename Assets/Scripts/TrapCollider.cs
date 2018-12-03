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
    void Start()
    {
        Invoke("CullExtra", 0.5f);
    }
    void CullExtra() { 
        Debug.Log("Created trap collider");
        bool found = false;
        var actual_tiles = new List<string>();
        foreach(Tilemap map in GameObject.FindObjectsOfType<Tilemap>()) {
            var cell = map.WorldToCell(this.transform.position);
            var tile = map.GetTile(cell);
            if (tile != null) {
                found |= KnownTraps.Contains(tile.name);
                actual_tiles.Add(tile.name);
            }
        }
        if (!found) {
            Debug.LogFormat("Extra trap collider removed {0}", System.String.Join(", ", actual_tiles.ToArray()));
            Destroy(this.gameObject);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
