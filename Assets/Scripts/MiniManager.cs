using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniManager : MonoBehaviour
{
    [SerializeField]
    Transform hand;

    [SerializeField]
    Transform parent;

    [SerializeField]
    GameObject room;

    [SerializeField]
    GameObject[] mini_items;

    // Use this for initialization
    private void OnEnable()
    {
        build_minature();
    }

    private void Update()
    {
        parent.position = hand.transform.position + Vector3.up * 0.08f;
        hand.gameObject.SetActive(false);
    }

    public void build_minature()
    {
        //Destroys Previous Objects
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject); //lol
        }

        for (int i = 0; i < room.transform.childCount; i++)
        {
            //Saves Object Type, Position, and Rotation
            GameObject obj = room.transform.GetChild(i).gameObject;
            if (obj.GetComponent<Item>() && obj.activeSelf)
            {
                Item item = obj.GetComponent<Item>();

                GameObject mini = null;

                if (item.object_type == "TV")
                    mini = Instantiate(mini_items[0]);
                else if (item.object_type == "CABINET")
                    mini = Instantiate(mini_items[1]);
                else if (item.object_type == "CHAIR")
                    mini = Instantiate(mini_items[2]);
                else if (item.object_type == "DESK1")
                    mini = Instantiate(mini_items[3]);
                else if (item.object_type == "DESK2")
                    mini = Instantiate(mini_items[4]);
                else if (item.object_type == "LOCKER")
                    mini = Instantiate(mini_items[5]);
                else if (item.object_type == "WHITEBOARD")
                    mini = Instantiate(mini_items[6]);

                mini.transform.parent = this.transform;
                mini.GetComponent<MiniItem>().Link(obj);
            }
        }
    }
}
