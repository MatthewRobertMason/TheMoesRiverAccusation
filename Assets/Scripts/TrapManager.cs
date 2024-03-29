﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapManager : MonoBehaviour {

    private List<List<GameObject>> all_traps = new List<List<GameObject>>();

    static T Pop<T>(List<T> list)
    {
        T item = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        return item;
    }


    private void Start()
    {
        Invoke("InitTraps", 0.1f);
    }

    private void InitTraps()
    {
        List<GameObject> traps = new List<GameObject>(GameObject.FindGameObjectsWithTag("Trap"));
        Debug.LogFormat("Found {0} trap components", traps.Count);

        while (traps.Count > 0) {
            List<GameObject> group = new List<GameObject>();
            group.Add(Pop(traps));
            bool changing = true;

            while (changing) {
                changing = false;
                for (int ii = 0; ii < traps.Count; ii++) {
                    for (int jj = 0; jj < group.Count; jj++) {
                        if ((group[jj].transform.position - traps[ii].transform.position).magnitude < 1.2) {
                            group.Add(traps[ii]);
                            traps.RemoveAt(ii);
                            changing = true;
                            ii = traps.Count;
                            break;
                        }
                    }
                }
            }
            all_traps.Add(group);
        }
        Debug.LogFormat("Found {0} traps", all_traps.Count);
    }
}
