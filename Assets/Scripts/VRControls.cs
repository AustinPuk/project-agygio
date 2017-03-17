using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For interfacing with Oculus Touch Controllers

public class VRControls : MonoBehaviour {    
    public static VRControls instance;

    public struct Hand
    {
        public bool triggerPressed;
        public bool triggerTouched;
        public bool gripPressed;
        public bool thumbTouch;     
    };

    public Hand right;
    public Hand left;

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
            right.gripPressed = true;
        else
            right.gripPressed = false;        
        if (OVRInput.Get(OVRInput.RawButton.RIndexTrigger))
            right.triggerPressed = true;
        else
            right.triggerPressed = false;
        if (OVRInput.Get(OVRInput.RawTouch.RIndexTrigger))
            right.triggerTouched = true;
        else
            right.triggerTouched = false;
        if (OVRInput.Get(OVRInput.RawNearTouch.RThumbButtons))
            right.thumbTouch = true;
        else
            right.thumbTouch = false;

        if (OVRInput.Get(OVRInput.RawButton.LHandTrigger))
            left.gripPressed = true;
        else
            left.gripPressed = false;
        if (OVRInput.Get(OVRInput.RawButton.LIndexTrigger))
            left.triggerPressed = true;
        else
            left.triggerPressed = false;
        if (OVRInput.Get(OVRInput.RawTouch.LIndexTrigger))
            left.triggerTouched = true;
        else
            left.triggerTouched = false;
        if (OVRInput.Get(OVRInput.RawNearTouch.LThumbButtons))
            left.thumbTouch = true;
        else
            left.thumbTouch = false;

        /*
        Debug.Log("RightGripPress: " + right.gripPressed);
        Debug.Log("RightThumbTouch: " + right.thumbTouch);
        Debug.Log("RightTriggerTouched: " + right.triggerTouched);
        */
    }
}
