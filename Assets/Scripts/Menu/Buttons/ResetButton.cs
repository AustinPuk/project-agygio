using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButton : MyButton
{

    public override void OnClick(PlayerHand hand)
    {
        WorldGenerator.instance.resetWorld = true;
    }
}
