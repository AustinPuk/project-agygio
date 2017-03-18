using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    [SerializeField]
    private float eatTime = 2.0f;

    [Tooltip("Amount of hunger that this item restores")]
    [SerializeField]
    private float amount;

    private bool isEating;
    private float timer;

    private void Start()
    {
        isEating = false;
        timer = eatTime;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "EatingRegion")
        {            
            if (!isEating)
            {
                isEating = true;
                // Start Particle System
                // Start Audio System
            }
            if (timer > 0.0f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                OnDrop(); // Drop Item first before destroying
                // Player.instance.consume(100);
                // Play Eat Audio
                Destroy(this);
            }            
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "EatingRegion")
        {
            isEating = false;
            timer = eatTime;

            // Stop Particle System
        }
    }

    public override void OnPress(){}

    public override void OnHold(){}

    public override void OnRelease(){}

    public override void OnPassive(){}
}
