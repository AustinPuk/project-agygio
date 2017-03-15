using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOfSphere : MonoBehaviour {

    [SerializeField]
    private Transform spawn_loc;

    [SerializeField]
    private GameObject spawn_me;

    [SerializeField]
    private GameObject spawn_mini;

    [SerializeField]
    private GameObject spawn_parent;

    [SerializeField]
    private GameObject spawn_mini_parent;

    public void OnClick()
    {
        GameObject new_spawn =  Instantiate(spawn_me);
        new_spawn.transform.position = spawn_loc.position;
        new_spawn.transform.rotation = spawn_loc.rotation;
        new_spawn.transform.parent = spawn_parent.transform;

        GameObject mini_spawn = Instantiate(spawn_mini);
        mini_spawn.transform.parent = spawn_mini_parent.transform;
        mini_spawn.GetComponent<MiniItem>().Link(new_spawn);
    }
}
