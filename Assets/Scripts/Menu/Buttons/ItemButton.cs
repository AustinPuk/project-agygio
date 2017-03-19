using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MyButton {

    // Button on item for when they are in backpack (not crafting)

    // Should highlight when items are selected

    private Item item;    

    private void Start()
    {
        item = transform.parent.GetComponent<Item>();
    }

    public override void OnClick()
    {
        // If item is not in storage, do nothing
        if (!item.inBackpack)
            return;
        else if (!item.isSelected)
        {
            Backpack.instance.SelectItem(item);
        }
        else
        {
            Backpack.instance.RemoveItem(item);
            item.OnGrab(VRControls.instance.rightHand);
        }        
    }
}
