using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightItem : Item {

    [Header("Light Parameters")]

    [SerializeField]
    private GameObject light;

    protected override void Update()
    {
        base.Update();
        if (isActive)
            light.SetActive(true);
        else if (!isActive)
            light.SetActive(false);
    }

    public override void OnPassive()
    {
        // Should be in here, but this is implemented incorrectly.
    }

    public override void OnHold()
    {
    }

    public override void OnPress()
    {
    }

    public override void OnRelease()
    {
    }
}
