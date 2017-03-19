using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHead : MonoBehaviour {

    [SerializeField]
    private Transform playerHead;

    [SerializeField]
    private float rate;

	// Use this for initialization
	void Start () {
	}
	
	void Update ()
    {
        Vector3 newLocation = new Vector3(playerHead.position.x, this.transform.position.y, playerHead.position.z);        
        Vector3 rotation = playerHead.rotation.eulerAngles;
        Quaternion newRotation = Quaternion.Euler(0.0f, rotation.y, 0.0f);
        //Debug.Log("Rot: " + rotation);  
        this.transform.position = Vector3.Lerp(this.transform.position, newLocation, rate);
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, newRotation, rate);
	}
}
