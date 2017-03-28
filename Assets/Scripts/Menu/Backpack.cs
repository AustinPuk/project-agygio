using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : Window {
    public static Backpack instance;

    [SerializeField]
    private Text number;

    void Awake()
    {                
        if (!instance)
            instance = this;
    }
		
    public override void AddItem(Item item)
    {        
        base.AddItem(item);

        // Debug.Log("Backpack: Add " + item.name);
        
        item.inBackpack = true;

        UpdateInventory(filterType);
    }

    public void RemoveItem(Item item)
    {
        //Debug.Log("Backpack: Remove " + item.name);

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

    public override void SelectItem(Item item)
    {
        base.SelectItem(item);
        number.text = countItem(item).ToString();
    }

    public override void Deselect()
    {
        base.Deselect();
        number.text = "";
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
    public void Clear()
    {
        Debug.Log("Clearing Inventory");

        for (int i = 0; i < items.Count; i++)
        {
            Item temp = items[i];
            items[i] = null;
            Destroy(temp.gameObject);
        }

        items.Clear();
        UpdateInventory(filterType);
    }
}
