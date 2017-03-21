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
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<Item>())
        {
            if (collision.gameObject.GetComponent<Item>().itemName == requiredTool.itemName)
            {
                Debug.Log("EnvObject Hit: Velocity " + Vector3.Magnitude(collision.relativeVelocity) + " " + this.name);
                if (Vector3.Magnitude(collision.relativeVelocity) > minVelocity)
                {
                    health -= 1 * Vector3.Magnitude(collision.relativeVelocity);
                    CheckHealth();
                }                    
            }
        }
    }

    private void CheckHealth()
    {
        if (health < 0.0f)
        {
            GameObject item = Instantiate(producedItem);
            item.transform.position = this.transform.position;

            // Play some sort of effect to indicate destroyed or hit

            Destroy(this.gameObject);
        }
    }
}
