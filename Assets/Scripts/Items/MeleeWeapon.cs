using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Item {

    [Header("Weapon Params")]

    [SerializeField]
    private float minimumVelocity = 0.0f;

    [SerializeField]
    private float baseDamage;

    [SerializeField]
    private Effects baseEffect;

    private Vector3 currentVelocity = new Vector3(0.0f, 0.0f, 0.0f);
    private Vector3 prevPosition = new Vector3(0.0f, 0.0f, 0.0f);
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Enemy>())
        {
            //Debug.Log("Weapon Found Enemy: " + Vector3.Magnitude(GetComponent<Rigidbody>().velocity));
            if (Vector3.Magnitude(currentVelocity) > minimumVelocity)
            {
                other.gameObject.GetComponent<Enemy>().TakeDamage(baseDamage, baseEffect);
            }
        }
    }

    protected override void Update()
    {
        base.Update();

        currentVelocity = (transform.position - prevPosition) / Time.deltaTime;
        prevPosition = transform.position;

        //Debug.Log("Current Velocity: " + currentVelocity);
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
