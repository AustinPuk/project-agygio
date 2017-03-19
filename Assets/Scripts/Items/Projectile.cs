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

    private bool isPlayers;

    private void OnTriggerEnter(Collider other)
    {
        if (!fired)
            return;

        if (other.gameObject.GetComponent<Enemy>())
        {
            if (isPlayers)
            {
                other.gameObject.GetComponent<Enemy>().TakeDamage(damage, damageType);
                Destroy(this.gameObject);
            }                
        }
        else if (other.gameObject.GetComponent<Player>())
        {
            if (!isPlayers)
            {
                other.gameObject.GetComponent<Player>().TakeDamage(damage, damageType);
                Destroy(this.gameObject);
            }                
        }
    }

    void Update()
    {
        if (fired)
        {
            transform.position = transform . position + (direction * speed * Time.deltaTime);
        }		
	}

    public void Fire(float dmg, Effects type, Vector3 dir, bool player)
    {
        fired = true;
        isPlayers = player;
        direction = dir;
        damage = dmg;
        damageType = type;
        Destroy(this.gameObject, lifetime);
    }
}
