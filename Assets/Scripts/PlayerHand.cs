using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {

    public bool isRightHand;

    public bool triggerPressed;
    public bool triggerTouched;
    public bool gripPressed;
    public bool thumbTouch;

    private Item heldObject;

    private float sphereOffset = 0.05f;

    private void OnTriggerStay(Collider collision)
    {
        // For Grabbing an Object

        if (heldObject)
            return;

        if (gripPressed)
        {
            if (collision.gameObject.tag == "GrabBox")
            {
                heldObject = collision.transform.parent.GetComponent<Item>();
                heldObject.OnGrab(this);                
            }
        }
    }
    
    private void Update ()
    {
        if (heldObject)
            GetComponent<Renderer>().enabled = false;
        else
            GetComponent<Renderer>().enabled = true;


        // Updates Controller Position/Rotation
        if (isRightHand)
        {
            this.transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
            this.transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
            //this.transform.localPosition += this.transform.forward * sphereOffset;
        }
        else
        {
            this.transform.localPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
            this.transform.localRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch);
            //this.transform.localPosition += this.transform.forward * sphereOffset;
        }


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
