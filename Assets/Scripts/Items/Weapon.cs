﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : Item {

    [SerializeField]
    private float minimumVelocity = 0.0f;

    [SerializeField]
    private float baseDamage;

    [SerializeField]
    private Effects baseEffect;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (Vector3.Magnitude(GetComponent<Rigidbody>().velocity) > minimumVelocity)
            {
                //collision.gameObject.GetComponent<Enemy>().takeDamage(baseDamage, baseEffect);
            }
        }
    }

    public override void OnPress()
    {
    }

    public override void OnHold()
    {
    }

    public override void OnRelease()
    {
    }

    public override void OnPassive()
    {
    }
}
