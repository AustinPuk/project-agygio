using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {

    public bool triggerPressed;
    public bool triggerTouched;
    public bool gripPressed;
    public bool thumbTouch;

    private Item heldObject;

    private void OnTriggerStay(Collider collision)
    {
        // For Grabbing an Object

        if (heldObject)
            return;

        if (gripPressed)
        {
            if (collision.gameObject.tag == "GrabItem")
            {
                heldObject = collision.transform.GetComponent<Item>();
                heldObject.OnGrab(this);
            }
            if (collision.gameObject.tag == "GrabBox")
            {
                heldObject = collision.transform.parent.GetComponent<Item>();
                heldObject.OnGrab(this);                
            }
        }
    }
    
    private void Update () {

        // For Dropping an Object

        if (!thumbTouch && !triggerTouched)
        {            
            if (heldObject)
            {
                heldObject.OnDrop();
                heldObject = null;
            }
        }		
	}
}
