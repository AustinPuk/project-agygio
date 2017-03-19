using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoringRegion : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {        
        if (other.gameObject.GetComponent<Item>())
        {
            Debug.Log("Item: Can Store");
            other.gameObject.GetComponent<Item>().canStore = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Item>())
        {
            Debug.Log("Item: No more store");
            other.gameObject.GetComponent<Item>().canStore = false;
        }
    }
}
