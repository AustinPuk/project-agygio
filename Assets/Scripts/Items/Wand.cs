using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wand : Item
{
    [Header("Wand Parameters")]

    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private Transform tipLoc;

    [SerializeField]
    private float minVelocity;

    [SerializeField]
    private float chargeMin;

    [SerializeField]
    private float chargeMax;

    [Header("Particle Stuff")]
    [SerializeField]
    private GameObject startParticles;

    [SerializeField]
    private GameObject chargedParticles;

    [Header("Damage Parameters")]

    [SerializeField]
    private float baseDamage;

    [SerializeField]
    private Effects baseEffect;

    private Vector3 tipVelocity;
    private Vector3 lastTiposition;

    private float chargeTimer;

    private void Start()
    {        
    }

    private void Shoot()
    {
        GameObject proj = Instantiate(projectile);
        proj.transform.position = tipLoc.position;
        proj.transform.rotation = tipLoc.rotation * proj.transform.rotation;
        proj.GetComponent<Projectile>().Fire(baseDamage, baseEffect, 
            Vector3.Lerp(Vector3.Normalize(tipVelocity), tipLoc.forward, 0.5f) * Vector3.Magnitude(tipVelocity), true, true);
        heldHand.SetHaptic(0.8f, 0.8f, 0.1f);
    }

    public override void OnPress()
    {
        lastTiposition = tipLoc.position;
        chargeTimer = chargeMax;
        startParticles.SetActive(true);
    }

    public override void OnHold()
    {
        tipVelocity = (tipLoc.position - lastTiposition) / Time.deltaTime;
        lastTiposition = tipLoc.position;

        if (chargeTimer > 0.0f)
        {
            heldHand.SetHaptic(0.6f, 0.3f);
            chargeTimer -= Time.deltaTime;
            if (chargeTimer < chargeMax - chargeMin && chargeTimer > 0.0f)
            {
                heldHand.SetHaptic(0.6f, 0.5f);
                chargedParticles.SetActive(true);
            }
        }
        else
        {
            heldHand.SetHaptic(0.0f, 0.0f);
            startParticles.SetActive(false);
            chargedParticles.SetActive(false);
        }
    }

    public override void OnRelease()
    {
        if (chargeTimer < chargeMax - chargeMin && chargeTimer > 0.0f)
        {
            if (Vector3.Magnitude(tipVelocity) > minVelocity)
                Shoot();
            else
                heldHand.SetHaptic(0.0f, 0.0f);
        }
        else
            heldHand.SetHaptic(0.0f, 0.0f);

        startParticles.SetActive(false);
        chargedParticles.SetActive(false);
    }

    public override void OnPassive()
    {
    }
}
