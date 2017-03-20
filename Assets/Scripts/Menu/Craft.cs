using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Craft : Window
{
    public static Craft instance;

    [SerializeField]
    private GameObject itemOneSpot;

    [SerializeField]
    private Text itemOneName;

    [SerializeField]
    private Text itemOneNumb;

    [SerializeField]
    private GameObject itemTwoSpot;

    [SerializeField]
    private Text itemTwoName;
    
    [SerializeField]
    private Text itemTwoNumb;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    public override void SelectItem(Item item)
    {
        base.SelectItem(item);
        
        if (!item.isDiscovered)
        {
            title.text = "???";
            description.text = "???";
        }

        if (item.requiredItemOne)
        {
            itemOneName.text = item.requiredItemOne.itemName;
            itemOneNumb.text = Backpack.instance.countItem(item.requiredItemOne) + "/" + item.numberItemOne.ToString();
        }
        else
        {
            itemOneName.text = "";
            itemOneNumb.text = "";
        }            
        if (item.requiredItemTwo)
        {
            itemTwoName.text = item.requiredItemTwo.itemName;
            itemTwoNumb.text = Backpack.instance.countItem(item.requiredItemTwo) + "/" + item.numberItemTwo.ToString();
        }
        else
        {
            itemTwoName.text = "";
            itemTwoNumb.text = "";
        }            
    }

    public void UpdateInfo()
    {
        if (!selectedItem)
            return;

        Item item = selectedItem;

        if (!item.isDiscovered)
        {
            title.text = "???";
            description.text = "???";
        }
        else
        {
            title.text = item.itemName;
            description.text = item.description;
        }

        if (item.requiredItemOne)
        {
            itemOneName.text = item.requiredItemOne.itemName;
            itemOneNumb.text = Backpack.instance.countItem(item.requiredItemOne) + "/" + item.numberItemOne.ToString();
        }
        else
        {
            itemOneName.text = "";
            itemOneNumb.text = "";
        }
        if (item.requiredItemTwo)
        {
            itemTwoName.text = item.requiredItemTwo.itemName;
            itemTwoNumb.text = Backpack.instance.countItem(item.requiredItemTwo) + "/" + item.numberItemTwo.ToString();
        }
        else
        {
            itemTwoName.text = "";
            itemTwoNumb.text = "";
        }
    }
}
