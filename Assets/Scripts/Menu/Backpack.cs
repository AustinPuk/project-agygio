using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Backpack : MonoBehaviour {
    public static Backpack instance;

    [SerializeField]
    private GameObject gridArea;

    [SerializeField]
    private GameObject hidden;

    [SerializeField]
    private Text title;

    [SerializeField]
    private Text description;

    [SerializeField]
    private float spacing;

    public List<Item> items;    

    private void Awake()
    {
        if (!instance)
            instance = this;
    }
	
	void Start ()
    {
        UpdateInventory();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AddItem(Item item)
    {
        Debug.Log("Backpack: Add " + item.name);

        if (!items.Contains(item))
            items.Add(item);

        item.inBackpack = true;

        UpdateInventory();

        item.transform.SetParent(hidden.transform);

        // Adjust Scale
        float newScale = spacing / Vector3.Magnitude(item.GetComponent<BoxCollider>().size);
        item.transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    public void RemoveItem(Item item)
    {
        Debug.Log("Backpack: Remove " + item.name);

        items.Remove(item);

        item.inBackpack = false;

        UpdateInventory();

        item.transform.SetParent(null);
        
        item.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public void SelectItem(Item item)
    {
        if (!items.Contains(item))
        {
            Debug.Log("Backpack: Error, selected item not in backpack");
            return;
        }

        // Update Info Sheet with Info from Item

        item.isSelected = true;

        title.text = item.itemName;
        description.text = item.description;
    }

    void UpdateInventory()
    {
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
        float width = Vector3.Distance(rightEdge, leftEdge);
        float height = Vector3.Distance(topEdge, botEdge);        

        Vector3 start = gridBox.bounds.center + ((topEdge - botEdge) * 0.5f) + ((leftEdge - rightEdge) * 0.5f);
        Vector3 spot = start;
        int row = 0;

        for (int i = 0; i < items.Count; i++)
        {
            //Horizontal Spacing
            if (i == 0)
                spot = spot + (spacing / 2.0f) * horizontalVector;
            else
                spot = spot + spacing * horizontalVector;
            //Vertical Spacing
            if (i == 0)
                spot = spot + (spacing / 2.0f) * verticalVector;
            else if (Vector3.Dot(spot - leftEdge, horizontalVector) >= width) //Checks if past bounds
            {
                //Start a new row
                row++;
                spot = start + (spacing / 2.0f) * horizontalVector;
                spot = spot + (spacing / 2.0f) * verticalVector;
                spot = spot + (spacing * verticalVector * row);
            }
            
            items[i].transform.position = spot;
            items[i].transform.SetParent(gridArea.transform);
        }
    }
}
