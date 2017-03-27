using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MyButton
{
    public override void OnClick(PlayerHand hand)
    {
        Player.instance.StartGame();
    }
}
