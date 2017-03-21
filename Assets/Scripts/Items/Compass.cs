using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : Item {

    [Header("Compass Parameters")]

    [SerializeField]
    private Transform target;

    [SerializeField]
    private LineRenderer line;

    [SerializeField]
    private float radius;

	// Use this for initialization
	void Start () {
        line.enabled = true;
	}

    private void DrawCompass(Vector3 dest)
    {
        Vector4 localStartPoint = new Vector4(line.GetPosition(0).x, line.GetPosition(0).y, line.GetPosition(0).z, 1);
        Vector3 startPoint = line.gameObject.transform.localToWorldMatrix * localStartPoint;
        Vector3 worldDirection = radius * Vector3.Normalize(dest - startPoint);
        Vector3 localDirection = line.gameObject.transform.worldToLocalMatrix * worldDirection;
        line.SetPosition(1, localDirection);        
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
        Vector3 center = new Vector3(0, WorldGenerator.instance.HeightLookup(0, 0), 0);
        if (!target)
            DrawCompass(center);
        else
            DrawCompass(target.position);
    }
}
