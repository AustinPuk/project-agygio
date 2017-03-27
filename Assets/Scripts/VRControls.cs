using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For interfacing with Steam VR Controllers

public class VRControls : MonoBehaviour {    
    public static VRControls instance;

    [SerializeField]
    SteamVR_ControllerManager manager;

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

        // STEAMVR
        SteamVR_TrackedController right = manager.right.GetComponent<SteamVR_TrackedController>();
        SteamVR_TrackedController left = manager.left.GetComponent<SteamVR_TrackedController>();
        if(right)
        {            
            rightHand.controllerIndex = (int)right.controllerIndex;
            if (right.triggerPressed)
                rightHand.triggerPressed = true;
            else
                rightHand.triggerPressed = false;
            if (right.gripped)
                rightHand.gripPressed = true;
            else
                rightHand.gripPressed = false;
            if (right.padPressed)
                rightHand.padPressed = true;
            else
                rightHand.padPressed = false;

        }

        if (left)
        {
            leftHand.controllerIndex = (int)left.controllerIndex;
            if (left.triggerPressed)
                leftHand.triggerPressed = true;
            else
                leftHand.triggerPressed = false;
            if (left.gripped)
                leftHand.gripPressed = true;
            else
                leftHand.gripPressed = false;
            if (left.padPressed)
                leftHand.padPressed = true;
            else
                leftHand.padPressed = false;
        }        
    }
}
