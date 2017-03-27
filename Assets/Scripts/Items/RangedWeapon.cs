using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Item
{
    [Header("Ranged Weapon Parameters")]

    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private Transform spawnLoc;

    [SerializeField]
    private float baseDamage;

    [SerializeField]
    private Effects baseEffect;
    
    public override void OnPress()
    {
        GameObject proj = Instantiate(projectile);
        proj.transform.position = spawnLoc.position;
        proj.transform.rotation = spawnLoc.rotation * proj.transform.rotation;
        proj.GetComponent<Projectile>().Fire(baseDamage, baseEffect, spawnLoc.forward, true);
        //Debug.DrawRay(spawnLoc.position, spawnLoc.forward, Color.red, 100.0f);
        heldHand.SetHaptic(0.8f, 0.1f);
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
