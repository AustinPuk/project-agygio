using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWindowButton : MyButton {

    [SerializeField]
    private GameObject window;

    [SerializeField]
    private bool opens;

    public override void OnClick()
    {
        window.SetActive(opens);
    }
}
