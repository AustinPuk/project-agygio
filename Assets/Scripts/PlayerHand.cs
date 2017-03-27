using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {

    [SerializeField]
    private LineRenderer pointer;

    [SerializeField]
    private Transform linkedHand;

    public bool isRightHand;

    public bool triggerPressed;
    public bool gripPressed;
    public bool padPressed;
    public int controllerIndex;

    private Item heldItem;
    private bool holdingTrigger; // Prevents multiple inputs when holding down button

    public Vector3 velocity;
    private Vector3 lastPos;
    
    private ushort hapticStrength;
    private IEnumerator hapticRoutine;

    private bool gripRefreshed;

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

        // Prevents multiple grip presses
        if (!gripPressed && !gripRefreshed)
            gripRefreshed = true;        

        // Updates Controller Position/Rotation
        this.transform.localPosition = linkedHand.localPosition;
        this.transform.localRotation = linkedHand.localRotation;        

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
        if (gripPressed && heldItem && gripRefreshed)
            dropItem();            
           
	}

    private void OnTriggerStay(Collider collision)
    {
        // For Grabbing an Object

        if (heldItem)
            return;

        if (collision.gameObject.tag == "GrabBox" || collision.gameObject.tag == "Handle")
            SetHaptic(0.2f);
        if (collision.gameObject.tag == "Handle" && holdWindow)
            SetHaptic(0.4f);

        if (gripPressed && gripRefreshed)
        {
            if (collision.gameObject.tag == "GrabBox")
            {                
                grabItem(collision.transform.parent.GetComponent<Item>());                
            }
            if (collision.gameObject.tag == "Handle" && !holdWindow)
            {                
                holdWindow = collision.transform.parent.parent.gameObject;                 
                holdWindow.transform.SetParent(this.transform);
            }
        }

        if(!gripPressed && holdWindow)
        {
            holdWindow.transform.SetParent(windowParent);
            holdWindow = null;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "GrabBox" || other.gameObject.tag == "Handle")
            SetHaptic(0.0f);
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
                        SetHaptic(0.5f, 0.1f);
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
            SetHaptic(0.5f, 0.1f);
            if(item.OnGrab(this))
                heldItem = item;
            gripRefreshed = false;
        }        
    }

    public void dropItem()
    {
        SetHaptic(0.5f, 0.1f);
        heldItem.OnDrop();
        heldItem = null;
        gripRefreshed = false;
    }

    public Item getHeldItem()
    {
        return heldItem;
    }

    /******************************** Haptic Functions ************************************/
 
    public void SetHaptic(float strength) 
    {
        // Assume strength is value between 0 - 1
        if (strength > 0)
        {
            hapticStrength = (ushort)Mathf.Lerp(0, 3999, strength);
            if (hapticRoutine == null)
            {
                hapticRoutine = Haptic();
                StartCoroutine(hapticRoutine);
            }
        }
        else
        {
            if (hapticRoutine != null)
            {
                StopCoroutine(hapticRoutine);
                hapticRoutine = null;
            }            
        }            
    }

    public void SetHaptic(float strength, float time)
    {
        // Assume strength is value between 0 - 1
        StartCoroutine(TimedHaptic(strength, time));
        return;        
    }
    
    private IEnumerator TimedHaptic(float strength, float time)
    {
        SetHaptic(strength);
        yield return new WaitForSeconds(time);
        SetHaptic(0);
        yield return null;
    }

    private IEnumerator Haptic() {
        while(true)
        {            
            SteamVR_Controller.Input(controllerIndex).TriggerHapticPulse(hapticStrength);
            yield return new WaitForSeconds(0.03f);
        }        
   }
}
