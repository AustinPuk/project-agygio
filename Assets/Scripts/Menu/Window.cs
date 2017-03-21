using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Window : MonoBehaviour
{
    [SerializeField]
    private GameObject UI;

    [SerializeField]
    private GameObject gridArea;

    [SerializeField]
    private GameObject hidden;

    [SerializeField]
    protected Text title;

    [SerializeField]
    protected Text description;

    [SerializeField]
    private float spacing;

    [SerializeField]
    private float offset;

    public List<Item> uniqueItems;
    public List<Item> items;

    public Item selectedItem;
    public ItemType filterType;

    protected virtual void Start()
    {
        foreach (Item item in items)
            AddItem(item);
        UpdateInventory(filterType);
    }
    
    protected virtual void Update()
    {

    }

    public virtual void AddItem(Item item)
    {
        if (!items.Contains(item))
            items.Add(item);

        item.transform.SetParent(hidden.transform);
        
        // Adjust Scale
        float newScale = spacing / Vector3.Magnitude(item.GetComponent<BoxCollider>().size);
        item.transform.localScale = new Vector3(newScale, newScale, newScale);
        item.transform.localRotation = Quaternion.Euler(item.storeRotate);
    }

    public virtual void SelectItem(Item item)
    {
        if (!items.Contains(item))
        {
            Debug.Log("Error, selected item not in backpack");
            return;
        }

        if (selectedItem)
            selectedItem.isSelected = false;

        selectedItem = item;
        item.isSelected = true;

        title.text = item.itemName;
        description.text = item.description;
    }

    public virtual void Deselect()
    {
        if (selectedItem)
            selectedItem.isSelected = false;
        selectedItem = null;
        title.text = "";
        description.text = "";
    }

    public void ChangeFilter(ItemType type)
    {
        filterType = type;
        UpdateInventory(filterType);
    }

    protected void UpdateInventory(ItemType filterType)
    {
        //Debug.Log("Updating " + this.name);

        ClearGrid();

        // Get bounds of grid region, despite rotations
        BoxCollider gridBox = gridArea.GetComponent<BoxCollider>();

        Vector3 localMax = gridBox.center + (gridBox.size / 2.0f);
        Vector3 localMin = gridBox.center - (gridBox.size / 2.0f);

        Vector3 leftEdge = gridBox.transform.TransformPoint(localMin.x, 0.0f, 0.0f);
        Vector3 rightEdge = gridBox.transform.TransformPoint(localMax.x, 0.0f, 0.0f);
        Vector3 topEdge = gridBox.transform.TransformPoint(0.0f, localMax.y, 0.0f);
        Vector3 botEdge = gridBox.transform.TransformPoint(0.0f, localMin.y, 0.0f);
        Vector3 horizontalVector = Vector3.Normalize(rightEdge - leftEdge);
        Vector3 verticalVector = Vector3.Normalize(botEdge - topEdge);
        Vector3 normalVector = Vector3.Normalize(Vector3.Cross(horizontalVector, verticalVector));
        float width = Vector3.Distance(rightEdge, leftEdge);
        //float height = Vector3.Distance(topEdge, botEdge);

        Vector3 start = gridBox.bounds.center + ((topEdge - botEdge) * 0.5f) + ((leftEdge - rightEdge) * 0.5f);
        Vector3 spot = start;
        int row = 0;

        bool addedFirstItem = false;

        List<Item> placedItems = new List<Item>();

        for (int i = 0; i < items.Count; i++)
        {
            if (filterType != ItemType.NONE && items[i].itemType != filterType)
                continue;

            bool checkDuplicate = false;
            foreach (Item item in placedItems)
            {
                // Don't place items if same item already on grid
                if (item.itemName == items[i].itemName)
                    checkDuplicate = true;
            }
            if (checkDuplicate)
                continue;
            

            //Horizontal Spacing
            if (!addedFirstItem)
                spot = spot + (spacing / 2.0f) * horizontalVector;
            else
                spot = spot + spacing * horizontalVector;
            //Vertical Spacing
            if (!addedFirstItem)
                spot = spot + (spacing / 2.0f) * verticalVector;
            else if (Vector3.Dot(spot - leftEdge, horizontalVector) >= width) //Checks if past bounds
            {
                //Start a new row
                row++;
                spot = start + (spacing / 2.0f) * horizontalVector;
                spot = spot + (spacing / 2.0f) * verticalVector;
                spot = spot + (spacing * verticalVector * row);
            }

            items[i].transform.position = spot + (normalVector * offset);
            items[i].transform.SetParent(gridArea.transform);

            placedItems.Add(items[i]);

            if (!addedFirstItem)
                addedFirstItem = true;
        }
    }

    protected void ClearGrid()
    {
        foreach (Item item in items)
        {
            if (item)
                item.transform.SetParent(hidden.transform);
        }
    }

    public void SetEnable(bool b)
    {
        UI.SetActive(b);
    }
}
