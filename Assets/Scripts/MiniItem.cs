using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniItem : Item {

    public GameObject linked_object;

    // Update is called once per frame
    void Update () {

        if (linked_object == null)
            return;

        if (selected)
        {
            linked_object.transform.localPosition = this.transform.localPosition;
            linked_object.transform.localRotation = this.transform.localRotation;
        }
        else if (linked_object.GetComponent<Item>().selected)
        {
            this.transform.localPosition = linked_object.transform.localPosition;
            this.transform.localRotation = linked_object.transform.localRotation;
        }
	}

    public void Link(GameObject obj)
    {
        //Debug.Log("Linking: " + name + " and " + obj.name);
        linked_object = obj;
        //Debug.Log("Truly Linking: " + name + " and " + linked_object.name);
        this.transform.localPosition = obj.transform.localPosition;
        this.transform.localRotation = obj.transform.localRotation;
        this.transform.localScale = obj.transform.localScale;

        this.constrain_x = obj.GetComponent<Item>().constrain_x;
        this.constrain_z = obj.GetComponent<Item>().constrain_z;
    }
}
