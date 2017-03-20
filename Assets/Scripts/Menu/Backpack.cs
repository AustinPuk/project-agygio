using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : Window {
    public static Backpack instance;

    void Awake()
    {                
        if (!instance)
            instance = this;
    }
		
    public override void AddItem(Item item)
    {        
        base.AddItem(item);

        Debug.Log("Backpack: Add " + item.name);
        
        item.inBackpack = true;

        UpdateInventory(filterType);
    }

    public void RemoveItem(Item item)
    {
        Debug.Log("Backpack: Remove " + item.name);

        items.Remove(item);

        item.inBackpack = false;
        item.isSelected = false;

        UpdateInventory(filterType);

        item.transform.SetParent(null);        
        
        item.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public void DeleteItem(string itemName)
    {        
        foreach(Item item in items)
        {
            if (item.itemName == itemName)
            {               
                items.Remove(item);
                Destroy(item.gameObject);
                UpdateInventory(filterType);
                return;
            }                
        }
    }

    public int countItem(Item item)
    {
        int result = 0;
        foreach (Item item2 in items)
        {
            if (item2.itemName == item.itemName)
                result++;
        }
        return result;
    }
}
