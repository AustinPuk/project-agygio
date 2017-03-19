using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowClick : MonoBehaviour {

    [SerializeField]
    private GameObject window;

    [SerializeField]
    private bool opens;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerHand>())
        {
            if (other.gameObject.GetComponent<PlayerHand>().triggerPressed)
                OnClick();
        }
    }

    void OnClick()
    {
        window.SetActive(opens);
    }
}
