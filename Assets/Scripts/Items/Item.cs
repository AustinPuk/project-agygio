using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public abstract class Item : MonoBehaviour
{
    public string itemName = "Test";
    public string description = "Test";

    [SerializeField]
    protected Transform grabLocation;

    [SerializeField]
    protected MeshRenderer highlight;

    [SerializeField]
    protected bool initialActiveState;
    
    [SerializeField]
    protected bool deactivateOnDrop;

    protected bool isActive;    
    protected PlayerHand heldHand;

    private bool holding; // If holding down the trigger

    public bool isHeld; // If currently being held by a hand
    public bool inBackpack;
    public bool canStore;    
    public bool isSelected; // If it is selected within the inventory

    public abstract void OnPress();
    public abstract void OnHold();
    public abstract void OnRelease();
    public abstract void OnPassive();    
    
    protected virtual void Awake()
    {
        isActive = initialActiveState;
        inBackpack = false;
        holding = false;
        canStore = false;        
    }

    protected virtual void Update()
    {
        if (isSelected)
            highlight.enabled = true;
        else
            highlight.enabled = false;

        if (inBackpack)
            return;

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
        if (inBackpack)
        {
            Debug.Log("Can't grab item in backpack");
            return;
        }

        Debug.Log("Grabbing");
        isActive = true;
        isHeld = true;
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

        isHeld = false;
        heldHand = null;
        transform.SetParent(null);

        if (canStore)
            Backpack.instance.AddItem(this);
        
        GetComponent<BoxCollider>().isTrigger = false;  // Allows item to fall to the floor
        //GetComponent<Rigidbody>().isKinematic = false;
    }    
}
