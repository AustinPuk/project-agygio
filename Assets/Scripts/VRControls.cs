using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For interfacing with Oculus Touch Controllers

public class VRControls : MonoBehaviour {    
    public static VRControls instance;

    [SerializeField]
    public PlayerHand rightHand;

    [SerializeField]
    public PlayerHand leftHand;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (OVRInput.Get(OVRInput.RawButton.RHandTrigger))
            rightHand.gripPressed = true;
        else
            rightHand.gripPressed = false;        
        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
            rightHand.triggerPressed = true;
        else
            rightHand.triggerPressed = false;
        if (OVRInput.Get(OVRInput.RawTouch.RIndexTrigger))
            rightHand.triggerTouched = true;
        else
            rightHand.triggerTouched = false;
        if (OVRInput.Get(OVRInput.RawNearTouch.RThumbButtons))
            rightHand.thumbTouch = true;
        else
            rightHand.thumbTouch = false;

        if (OVRInput.Get(OVRInput.RawButton.LHandTrigger))
            leftHand.gripPressed = true;
        else
            leftHand.gripPressed = false;
        if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
            leftHand.triggerPressed = true;
        else
            leftHand.triggerPressed = false;
        if (OVRInput.Get(OVRInput.RawTouch.LIndexTrigger))
            leftHand.triggerTouched = true;
        else
            leftHand.triggerTouched = false;
        if (OVRInput.Get(OVRInput.RawNearTouch.LThumbButtons))
            leftHand.thumbTouch = true;
        else
            leftHand.thumbTouch = false;

        /*
        Debug.Log("RightGripPress: " + right.gripPressed);
        Debug.Log("RightThumbTouch: " + right.thumbTouch);
        Debug.Log("RightTriggerTouched: " + right.triggerTouched);
        */
    }
}
