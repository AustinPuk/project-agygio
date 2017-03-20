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

    public override void OnClick(PlayerHand hand)
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
            Item heldItem = hand.getHeldItem();

            if (heldItem)
            {
                // Switches item if there is an item already in the player's hand
                hand.dropItem();
                Backpack.instance.AddItem(heldItem);
            }

            Backpack.instance.RemoveItem(item);
            hand.grabItem(item);            
        }        
    }
}
