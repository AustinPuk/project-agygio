﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {

    [SerializeField]
    private LineRenderer pointer;

    public bool isRightHand;

    public bool triggerPressed;
    public bool triggerTouched;
    public bool gripPressed;
    public bool thumbTouch;

    private Item heldObject;

    LayerMask windowOnly;
    LayerMask buttonsOnly;

    private void Start()
    {
        windowOnly = 1 << LayerMask.NameToLayer("Window");
        buttonsOnly = 1 << LayerMask.NameToLayer("Buttons");
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


        // Menu Pointer for Interactions
        if (Menu.isOpen)
            WindowPointer();
        else
            pointer.enabled = false;


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
                if (!heldObject.isHeld) // Makes sure object not already in other hand
                    heldObject.OnGrab(this);
            }
        }
    }

    private void WindowPointer()
    {
        // Raycast

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 100.0f, windowOnly))
        {
            // If it hits window, render line and check for trigger
            GetComponent<Renderer>().enabled = false;            
            pointer.SetPosition(0, transform.position);
            pointer.SetPosition(1, hit.point);
            pointer.enabled = true;

            if (this.triggerPressed)
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, 100.0f, buttonsOnly))
                {
                    if (hit.collider.gameObject.GetComponent<MyButton>())
                    {
                        Debug.Log("Pressing Button " + hit.collider.gameObject.name);
                        hit.collider.gameObject.GetComponent<MyButton>().OnClick();
                    }

                }
            }


        }
        else
        {
            pointer.enabled = false;
        }
        
    }
}
