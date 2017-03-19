using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilterButton : MyButton {

    [SerializeField]
    private GameObject window;

    [SerializeField]
    private ItemType type;

    public override void OnClick()
    {
        window.GetComponent<Backpack>().ChangeFilter(type);
    }
}
