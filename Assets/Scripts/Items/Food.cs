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

    [SerializeField]
    ParticleSystem particles;

    private bool isEating;
    private float timer;

    private void Start()
    {
        isEating = false;
        timer = eatTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isActive)
            return;

        if (other.gameObject.tag == "EatingRegion")
        {            
            if (!isEating)
            {
                Debug.Log("Food eating");
                isEating = true;
                timer = eatTime;
                // Start Particle System
                particles.Play();
                // Start Audio System
            }
            if (timer > 0.0f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Food has been eaten");
                OnDrop(); // Drop Item first before destroying
                Player.instance.Eat(amount);                
                // Play Eat Audio
                Destroy(this.gameObject);
            }            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isActive)
            return;

        if (other.gameObject.tag == "EatingRegion")
        {
            isEating = false;
            timer = eatTime;

            // Stop Particle System
            particles.Stop();
        }
    }

    public override void OnPress(){}

    public override void OnHold(){}

    public override void OnRelease(){}

    public override void OnPassive(){}
}
