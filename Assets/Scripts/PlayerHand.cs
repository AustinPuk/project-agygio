using System.Collections;
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

    private Item heldItem;
    private bool holdingTrigger; // Prevents multiple inputs when holding down button

    public Vector3 velocity;
    private Vector3 lastPos; 

    // Hacky window moving
    private GameObject holdWindow;    
    [SerializeField]
    private Transform windowParent;

    LayerMask windowOnly;
    LayerMask buttonsOnly;

    private void Start()
    {
        windowOnly = 1 << LayerMask.NameToLayer("Window");
        buttonsOnly = 1 << LayerMask.NameToLayer("Buttons");
        lastPos = transform.position;        
    }    
        
    
    private void Update ()
    {
        // Calculates velocity for throwing and stuff
        velocity = (transform.position - lastPos) / Time.deltaTime;
        lastPos = transform.position;

        // Hand isn't rendered when holding objects
        if (heldItem)
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


        // Checks if trigger is being held down (must be after windowPoitner)
        if (triggerPressed && !holdingTrigger)
            holdingTrigger = true;
        if (holdingTrigger && !triggerPressed)
            holdingTrigger = false;


        // For Dropping an Object

        if (!gripPressed && !thumbTouch && !triggerTouched)
        {
            if (heldItem)
            {                
                dropItem();
            }
        }
	}

    private void OnTriggerStay(Collider collision)
    {
        // For Grabbing an Object

        // TODO: Be able to detect "pressing" action, rather than checking if it's held down.

        if (heldItem)
            return;

        if (collision.gameObject.tag == "GrabBox" || collision.gameObject.tag == "Handle")
            SetHaptic(0.2f, 0.2f);

        if (gripPressed)
        {
            if (collision.gameObject.tag == "GrabBox")
            {                
                grabItem(collision.transform.parent.GetComponent<Item>());                
            }
            if (collision.gameObject.tag == "Handle" && !holdWindow)
            {                
                // Ugly Handle Code 
                holdWindow = collision.transform.parent.parent.gameObject;                 
                holdWindow.transform.SetParent(this.transform);
            }
        }

        if(!gripPressed && holdWindow)
        {
            SetHaptic(0.0f, 0.0f);
            holdWindow.transform.SetParent(windowParent);
            holdWindow = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "GrabBox" || other.gameObject.tag == "Handle")
            SetHaptic(0.0f, 0.0f);
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

            if (!holdingTrigger && this.triggerPressed)
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, 100.0f, buttonsOnly))
                {
                    if (hit.collider.gameObject.GetComponent<MyButton>())
                    {
                        //Debug.Log("Pressing Button " + hit.collider.gameObject.name);
                        SetHaptic(0.4f, 0.4f, 0.1f);
                        hit.collider.gameObject.GetComponent<MyButton>().OnClick(this);                        
                    }
                }
            }
        }
        else
        {
            pointer.enabled = false;
        }
        
    }

    public void grabItem(Item item)
    {
        if (!item.isHeld)
        {
            SetHaptic(0.5f, 0.5f, 0.1f);
            if(item.OnGrab(this))
                heldItem = item;
        }        
    }

    public void dropItem()
    {
        SetHaptic(0.2f, 0.4f, 0.1f);
        heldItem.OnDrop();
        heldItem = null;
    }

    public Item getHeldItem()
    {
        return heldItem;
    }
        
    public void SetHaptic(float frequency, float amplitude)
    {        
        if (isRightHand)
            OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.RTouch);
        else
            OVRInput.SetControllerVibration(frequency, amplitude, OVRInput.Controller.LTouch);
    }

    public void SetHaptic(float frequency, float amplitude, float time)
    {
        StartCoroutine(TimedHaptic(frequency, amplitude, time));
    }

    private IEnumerator TimedHaptic(float frequency, float amplitude, float time)
    {
        SetHaptic(frequency, amplitude);
        yield return new WaitForSeconds(time);
        SetHaptic(0.0f, 0.0f);
        yield return null;
    }
}
