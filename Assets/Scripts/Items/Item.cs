using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    //public string name = "Test";

    [SerializeField]
    protected bool initialActiveState;
    
    [SerializeField]
    protected bool deactivateOnDrop;    

    protected bool isActive;    
    protected VRControls.Hand heldHand;

    public abstract void OnPress();
    public abstract void OnHold();
    public abstract void OnRelease();
    public abstract void OnPassive();
    
    protected virtual void Awake()
    {
        isActive = initialActiveState;
    }

    protected virtual void Update()
    {
        OnPassive();
    }

    public virtual void OnGrab(VRControls.Hand hand)
    {
        isActive = true;
        this.heldHand = hand;
        this.transform.SetParent(hand.transform);
    }

    public virtual void OnDrop()
    {
        if (deactivateOnDrop)
            isActive = false;
        transform.SetParent(null);
    }    
}
