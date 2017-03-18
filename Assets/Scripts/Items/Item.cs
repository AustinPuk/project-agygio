using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public abstract class Item : MonoBehaviour
{
    //public string name = "Test";

    [SerializeField]
    protected Transform grabLocation;

    [SerializeField]
    protected bool initialActiveState;
    
    [SerializeField]
    protected bool deactivateOnDrop;    

    protected bool isActive;    
    protected PlayerHand heldHand;

    private bool holding;

    public abstract void OnPress();
    public abstract void OnHold();
    public abstract void OnRelease();
    public abstract void OnPassive();    
    
    protected virtual void Awake()
    {
        isActive = initialActiveState;
        holding = false;
    }

    protected virtual void Update()
    {
        if (isActive)
            OnPassive();

        if (!heldHand)
            return;

        if (!holding && heldHand.triggerPressed)
        {
            OnPress();
            holding = true;
        }
            
        if (heldHand.triggerPressed)
            OnHold();


        if (holding && !heldHand.triggerPressed)
        {
            OnRelease();
            holding = false;
        }
    }

    public virtual void OnGrab(PlayerHand hand)
    {
        Debug.Log("Grabbing");
        isActive = true;
        heldHand = hand;
        transform.SetParent(hand.gameObject.transform);
        transform.localPosition = grabLocation.localPosition;
        transform.localRotation = grabLocation.localRotation;
        
        GetComponent<BoxCollider>().isTrigger = true;   // Prevents item from pushing back items and stuff. 
        GetComponent<Rigidbody>().isKinematic = true;   // Prevents item from being affected by gravity

    }

    public virtual void OnDrop()
    {
        Debug.Log("Dropping");
        if (deactivateOnDrop)
            isActive = false;

        heldHand = null;
        transform.SetParent(null);
        
        GetComponent<BoxCollider>().isTrigger = false;  // Allows item to fall to the floor
        //GetComponent<Rigidbody>().isKinematic = false;
    }    
}
