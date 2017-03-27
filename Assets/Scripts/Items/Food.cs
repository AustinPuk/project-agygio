using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item
{
    [Header("Food Parameters")]

    [SerializeField]
    private float eatTime = 1.3f;

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

    protected override void Update()
    {
        base.Update();
        if (inBackpack && isEating)
        {
            isEating = false;
            particles.Stop();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isActive || inBackpack)
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

                if(heldHand)
                    heldHand.SetHaptic(0.6f);
            }
            if (timer > 0.0f)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                Debug.Log("Food has been eaten");

                if (heldHand)
                    heldHand.SetHaptic(0.0f);

                OnDrop(); // Drop Item first before destroying
                Player.instance.Eat(amount);

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
            if(heldHand)
                heldHand.SetHaptic(0.0f);

            // Stop Particle System
            particles.Stop();
        }
    }

    public override void OnPress(){}

    public override void OnHold(){}

    public override void OnRelease(){}

    public override void OnPassive(){}
}
