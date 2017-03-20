using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftButton : MyButton {    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void OnClick(PlayerHand hand)
    {
        bool canCraft = false;
        Item item = Craft.instance.selectedItem;
        if (item)
        {
            if (item.requiredItemOne && 
                Backpack.instance.countItem(item.requiredItemOne) >= item.numberItemOne)
            {
                Debug.Log("Crafting: One Item Good");
                if (item.requiredItemTwo)
                {
                    if (Backpack.instance.countItem(item.requiredItemTwo) >= item.numberItemTwo)
                    {
                        Debug.Log("Crafting: Two Item Good");
                        // Have enough of both items. Get rid, and add
                        for (int i = 0; i < item.numberItemOne; i++)
                            Backpack.instance.DeleteItem(item.requiredItemOne.itemName);
                        for (int i = 0; i < item.numberItemTwo; i++)
                            Backpack.instance.DeleteItem(item.requiredItemTwo.itemName);
                        canCraft = true;
                    }
                }
                else
                {
                    // Have enough of the one item you need
                    for (int i = 0; i < item.numberItemOne; i++)
                        Backpack.instance.DeleteItem(item.requiredItemOne.itemName);
                    canCraft = true;
                }
            }
        }

        if (canCraft)
        {
            Debug.Log("Crafting " + item.itemName);
            // Set Craft Item to Discovered
            item.isDiscovered = true;

            // Create new item object
            GameObject newItem = Instantiate(item.gameObject);

            newItem.transform.SetParent(null);
            newItem.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            newItem.GetComponent<Item>().craftMenuItem = false;
            newItem.GetComponent<Item>().isSelected = false;

            // Equip to player's hand
            Item heldItem = hand.getHeldItem();

            if (heldItem)
            {
                // Switches item if there is an item already in the player's hand
                hand.dropItem();
                Backpack.instance.AddItem(heldItem);
            }

            hand.grabItem(newItem.GetComponent<Item>());

            Craft.instance.UpdateInfo();
        }
        else
        {
            Debug.Log("Crafting Failed");
        }
    }
}
