using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public abstract class MyButton : MonoBehaviour {

    public bool testClick;

    private void Update()
    {
        if (testClick)
        {
            OnClick(VRControls.instance.rightHand);
            testClick = false;
        }
    }

    public abstract void OnClick(PlayerHand hand);
}
