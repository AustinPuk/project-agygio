﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWindowButton : MyButton {

    [SerializeField]
    private GameObject window;

    [SerializeField]
    private GameObject playerHead;

    [SerializeField]
    private float spawnDist;

    [SerializeField]
    private bool opens;

    public override void OnClick(PlayerHand hand)
    {
        if (window.GetComponent<Backpack>())
        {
            window.GetComponent<Backpack>().SetEnable(opens);

            if (opens)
            {
                Vector3 spawnLoc = playerHead.transform.position + playerHead.transform.forward * spawnDist;
                spawnLoc.y = window.transform.position.y;
                window.transform.position = spawnLoc;
                window.transform.LookAt((window.transform.position - playerHead.transform.position) + window.transform.position);
            }            
        }
            
    }
}
