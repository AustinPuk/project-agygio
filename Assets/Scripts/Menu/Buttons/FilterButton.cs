using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterButton : MyButton {

    [SerializeField]
    private Window window;

    [SerializeField]
    private ItemType type;

    public override void OnClick(PlayerHand hand)
    {
        window.ChangeFilter(type);
    }
}
