using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemCollider : MonoBehaviour {

    public int collisions;

    [SerializeField]
    string item_collider_tag;

    [SerializeField]
    string wall_tag;

    void Start()
    {
        collisions = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == item_collider_tag)
            collisions++;
        if (other.gameObject.tag == wall_tag)
            collisions++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == item_collider_tag)
            collisions--;
        if (other.gameObject.tag == wall_tag)
            collisions--;
    }
}
