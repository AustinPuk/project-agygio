using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public abstract class Item : MonoBehaviour
{    
    public string itemName = "Test";
    public ItemType itemType;
    public string description = "Test";

    [SerializeField]
    protected Transform grabLocation;    

    [SerializeField]
    protected MeshRenderer highlight;

    [SerializeField]
    public Transform storeRotate;

    [SerializeField]
    protected bool initialActiveState;
    
    [SerializeField]
    protected bool deactivateOnDrop;

    [Header("Crafting")]
    public bool isDiscovered;
    public bool craftMenuItem;
    public Item requiredItemOne;
    public int numberItemOne;
    public Item requiredItemTwo;
    public int numberItemTwo;

    [Header("Debug View")]
    public bool isHeld; // If currently being held by a hand
    public bool inBackpack;
    public bool canStore;    
    public bool isSelected; // If it is selected within the inventory

    protected bool isActive;
    protected PlayerHand heldHand;
    private bool holding; // If holding down the trigger

    public abstract void OnPress();
    public abstract void OnHold();
    public abstract void OnRelease();
    public abstract void OnPassive();    
    
    protected virtual void Awake()
    {
        isActive = initialActiveState;
        inBackpack = false;
        isDiscovered = false;
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

    public virtual bool OnGrab(PlayerHand hand)
    {
        if (inBackpack || craftMenuItem)
        {
            Debug.Log("Can't grab item");
            return false;
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

        return true;

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
