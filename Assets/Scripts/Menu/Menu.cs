using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {

    public static bool isOpen;

    [SerializeField]
    private GameObject scroll;

    [SerializeField]
    private Transform pouch;

    float currentDist;
    private Vector3 originalPos;

    private GameObject hand;
   
    // Use this for initialization
    void Start ()
    {
        originalPos = transform.localPosition;
        currentDist = 0.0f;
        isOpen = false;
	}

    private void Update()
    {
        if (!hand)
            return;

        if (hand.GetComponent<PlayerHand>().gripPressed && !hand.GetComponent<PlayerHand>().getHeldItem())
        {
            currentDist = Vector3.Dot(hand.transform.position - pouch.position, -pouch.right);
            currentDist = Mathf.Clamp(currentDist, 0.05f, 0.47f);
            this.transform.position = (currentDist * -pouch.right) + pouch.position;
            hand.GetComponent<PlayerHand>().SetHaptic(0.2f, 0.2f);
        }
        else
        {
            if (currentDist > 0.44f)
            {
                //Debug.Log("Opening Scroll");
                scroll.SetActive(true);
                isOpen = true;
                hand.GetComponent<PlayerHand>().SetHaptic(0.4f, 0.4f, 0.08f);
            }
            else
            {
                //Debug.Log("Closing Scroll");
                scroll.SetActive(false);
                this.transform.localPosition = originalPos;
                isOpen = false;
                Backpack.instance.Deselect();
                Craft.instance.Deselect();
                Backpack.instance.SetEnable(false);
                Craft.instance.SetEnable(false);
                hand.GetComponent<PlayerHand>().SetHaptic(0.2f, 0.4f, 0.08f);
            }
            hand = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerHand>())
        {
            if (!hand && other.gameObject.GetComponent<PlayerHand>().gripPressed)
            {
                if (!other.gameObject.GetComponent<PlayerHand>().getHeldItem())
                    hand = other.gameObject;                
            }                
        }
    }    
}
