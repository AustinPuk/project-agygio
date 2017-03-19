using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public abstract class MyButton : MonoBehaviour {

    public abstract void OnClick();
}
