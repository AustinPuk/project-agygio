using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campfire : MonoBehaviour
{
    public static Campfire instance;

    public bool isLit;

    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private float depleteRate; // In Minutes

    [SerializeField]
    private ParticleSystem fire;

    [SerializeField]
    private Light lit;

    public float health;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    // Use this for initialization
    void Start() {
        ResetCampfire();
    }

    public void ResetCampfire()
    {
        TurnFireOff();
        float y = WorldGenerator.instance.HeightLookup(transform.position.x, transform.position.z);
        transform.position = new Vector3(transform.position.x, y + 0.1f, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Item>())
        {
            Item item = other.GetComponent<Item>();
            if (item.itemName == "Torch")
            {
                if (!isLit)
                {
                    LightFire();
                }
            }
            else if (item.itemName == "Wood" || item.itemName == "Stick" || item.itemName == "Grass")
            {
                if (isLit)
                    HealFire(item);
            }           
        }
    }

    // Update is called once per frame
    void Update () {

        if (isLit)
        {
            health = Mathf.Clamp(health - (depleteRate / 60.0f) * Time.deltaTime, 0, maxHealth);

            if (health <= 0)
                TurnFireOff();

            ParticleSystem.EmissionModule em = fire.emission;
            em.rateOverTime = health;
        }		
	}

    private void HealFire(Item item)
    {
        float amount = 0.0f;
        if (item.itemName == "Wood")
            amount = 20.0f;
        else if (item.itemName == "Stick")
            amount = 5.0f;
        else
            amount = 3.0f;

        health = Mathf.Clamp(health + amount, 0, maxHealth);

        item.canStore = false;
        item.OnDrop();
        Destroy(item.gameObject);
    }

    private void LightFire()
    {        
        fire.Play();
        lit.enabled = true;
        isLit = true;
        health = maxHealth / 3.0f;
    }

    private void TurnFireOff()
    {
        fire.Stop();
        lit.enabled = false;
        isLit = false;
        health = 0.0f;
    }


}
