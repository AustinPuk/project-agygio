using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Destroy(this);
            //collision.gameObject.GetComponent<PlayerController>().eat(102);
        }
    }

    public override void TriggerUse()
    {
        throw new NotImplementedException();
    }

    public override void HoldUse()
    {
        throw new NotImplementedException();
    }

    public override void CollisionUse()
    {
        throw new NotImplementedException();
    }

    public override void PassiveUse()
    {
        throw new NotImplementedException();
    }
}
