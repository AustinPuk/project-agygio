using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
public class EnvironmentObject : MonoBehaviour {

    [SerializeField]
    private GameObject producedItem;

    [SerializeField]
    private Item requiredTool;

    [SerializeField]
    private float health;

    [SerializeField]
    private float minVelocity;

    private void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<MeleeWeapon>())
        {
            if (other.gameObject.GetComponent<MeleeWeapon>().itemName == requiredTool.itemName)
            {
                float vel = Vector3.Magnitude(other.gameObject.GetComponent<MeleeWeapon>().currentVelocity);
                // Debug.Log("EnvObject Hit: Velocity " + vel + " " + this.name);
                if (vel > minVelocity)
                {
                    health -= 1 * vel;
                    CheckHealth();
                    other.gameObject.GetComponent<Item>().heldHand.SetHaptic(0.8f, 0.1f); // TODO : Change Protection Level Back
                }
                else
                {
                    other.gameObject.GetComponent<Item>().heldHand.SetHaptic(0.2f, 0.1f);
                }
            }
        }
    }

    private void CheckHealth()
    {
        if (health < 0.0f)
        {
            GameObject item = Instantiate(producedItem);
            item.transform.position = this.transform.position + (Vector3.up * 1.0f);
            item.GetComponent<Rigidbody>().AddForce(Vector3.up * 20.0f);

            // Play some sort of effect to indicate destroyed or hit
            Destroy(this.gameObject);
        }
    }
}
