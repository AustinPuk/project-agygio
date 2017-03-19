using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenInventory : MonoBehaviour {

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
	}

    private void Update()
    {
        if (!hand)
            return;

        if (hand.GetComponent<PlayerHand>().triggerPressed)
        {
            currentDist = Vector3.Dot(hand.transform.position - pouch.position, -pouch.right);
            currentDist = Mathf.Clamp(currentDist, 0.05f, 0.47f);
            this.transform.position = (currentDist * -pouch.right) + pouch.position;
        }
        else
        {
            if (currentDist > 0.44f)
            {
                Debug.Log("Opening Scroll");
                scroll.SetActive(true);
            }
            else
            {
                Debug.Log("Closing Scroll");
                scroll.SetActive(false);
                this.transform.localPosition = originalPos;
            }
            hand = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerHand>())
        {
            if (!hand && other.gameObject.GetComponent<PlayerHand>().triggerPressed)
                hand = other.gameObject;
        }
    }    
}
