using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [SerializeField]
    private float speed;

    [SerializeField]
    private float lifetime;

    private bool fired = false;

    private Vector3 direction;
    private Effects damageType;
    private float damage;

    private bool thrown;

    private bool isPlayers;

    private void OnTriggerEnter(Collider other)
    {
        if (!fired)
            return;

        // Debug.Log("Fire hitting " + other.name);

        if (other.gameObject.GetComponent<Enemy>())
        {
            Debug.Log("Fire hit enemy");
            if (isPlayers)
            {
                //Debug.Log("Damaging Enemy");
                other.gameObject.GetComponent<Enemy>().TakeDamage(damage, damageType);
                Destroy(this.gameObject);
            }                
        }
        else if (other.gameObject.GetComponent<Player>())
        {            
            if (!isPlayers)
            {
                other.gameObject.GetComponent<Player>().TakeDamage(damage, direction, damageType);
                Destroy(this.gameObject);
            }                
        }
    }

    void Update()
    {
        if (fired && !thrown)
        {
            transform.position = transform . position + (direction * speed * Time.deltaTime);
        }		
	}

    public void Fire(float dmg, Effects type, Vector3 dir, bool player, bool magic = false)
    {
        if (magic)
        {
            this.GetComponent<Rigidbody>().isKinematic = false;
            this.GetComponent<Rigidbody>().AddForce(dir.x * 2.0f, dir.y * 2.0f, dir.z * 2.0f, ForceMode.Impulse);
            thrown = true;
        }
        else
            thrown = false; 

        fired = true;
        isPlayers = player;
        direction = dir;
        damage = dmg;
        damageType = type;
        Destroy(this.gameObject, lifetime);
    }
}
