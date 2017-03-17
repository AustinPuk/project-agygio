using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour {

    //public string name = "Test";

    protected bool isActive;
    protected VRControls.Hand heldHand;

    public abstract void TriggerUse();
    public abstract void HoldUse();
    public abstract void CollisionUse();
    public abstract void PassiveUse();

    public virtual void OnGrab(Transform parent, VRControls.Hand hand)
    {
        isActive = true;
        this.heldHand = hand;
        this.transform.SetParent(parent);
    }

    public virtual void OnDrop()
    {
        isActive = false;
        transform.SetParent(null);
    }    
}
