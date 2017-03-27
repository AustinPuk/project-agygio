using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public static Player instance;

    [SerializeField]
    public float maxHealth;

    [SerializeField]
    public float maxHunger;

    [SerializeField]
    private GameObject damageIndicator;

    [Tooltip("Rate that hunger reduces per MINUTE")]
    [SerializeField]
    private float hungerRate;

    [Tooltip("Rate that health restores over time. Actual rate changes proportional to hunger")]
    [SerializeField]
    private float baseHealthRegen;

    public float health;
    public float hunger;

    void Awake()
    {
        if (!instance)
            instance = this;
    }

	// Use this for initialization
	void Start () {
        health = maxHealth;
        hunger = maxHunger;
	}
	
	// Update is called once per frame
	void Update () {

        Regen(Time.deltaTime);
        CheckDeath();
		        
	}

    /**************************** Player Updates ***********************************/

    void Regen(float deltaTime)
    {
        hunger = Mathf.Clamp(hunger  - (hungerRate / 60.0f) * deltaTime, 0, maxHunger);
        //health += hunger * some logarithmic modifier * baseHealthRegen;
        float regen = (baseHealthRegen / 60.0f) * (((2 * hunger) / maxHunger) - 1.0f);
        health = Mathf.Clamp(health + (regen * Time.deltaTime), 0.0f, maxHealth);
    }

    void CheckDeath()
    {
        if (health <= 0.0f)
            OnDeath();        
    }

    /***************************** Player Event Functions *************************/
    void OnDeath()
    {
        //Debug.Log("Player Dies");
        // Dies
        // Some sort of method for transitioning to a main menu / title screen / DARKNESS
    }

    IEnumerator KnockBack(Vector3 direction)
    {        
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().AddForce(direction * 300.0f);
        damageIndicator.SetActive(true);
        yield return new WaitForSeconds(0.09f);
        GetComponent<Rigidbody>().isKinematic = true;
        FixHeight();
        damageIndicator.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        damageIndicator.SetActive(false);
        yield return null;
    }


    /***************************** Public Functions *******************************/

    public void Eat(float amount)
    {
        //Debug.Log("Ate " + amount);
        hunger = Mathf.Clamp(hunger + amount, 0.0f, maxHunger);
    }    

    public void TakeDamage(float amount, Vector3 dir, Effects type)
    {
        //Debug.Log("Player: Take Damage");
        health -= amount;

        VRControls.instance.rightHand.SetHaptic(1.0f, 0.2f);
        VRControls.instance.leftHand.SetHaptic(1.0f, 0.2f);

        dir.y = 0.0f;
        StartCoroutine(KnockBack(dir));

        // TODO: take type into account, and possibly even armor. 
    }    

    /******************************* Helper Functions **********************************/

    private void FixHeight() 
    {
        float x = transform.position.x;
        float z = transform.position.z;
        transform.position = new Vector3(x, WorldGenerator.instance.HeightLookup(x, z), z);
    }
}
