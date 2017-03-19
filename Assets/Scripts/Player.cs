using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    public static Player instance;

    [SerializeField]
    private float maxHealth;

    [SerializeField]
    private float maxHunger;

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
		        
	}

    /**************************** Player Updates ***********************************/

    void Regen(float deltaTime)
    {
        hunger = Mathf.Clamp(hunger  - (hungerRate / 60.0f) * deltaTime, 0, maxHunger);
        //health += hunger * some logarithmic modifier * baseHealthRegen;
        health = Mathf.Clamp(health + baseHealthRegen / 60.0f, 0.0f, maxHealth);
    }

    void CheckDeath()
    {
        if (health <= 0.0f)
            OnDeath();        
    }

    /***************************** Player Event Functions *************************/
    void OnDeath()
    {
        Debug.Log("Player Dies");
        // Dies
        // Some sort of method for transitioning to a main menu / title screen / DARKNESS
    }


    /***************************** Public Functions *******************************/

    public void Eat(float amount)
    {
        Debug.Log("Ate " + amount);
        hunger = Mathf.Clamp(hunger + amount, 0.0f, maxHunger);
    }    

    public void TakeDamage(float amount, Effects type)
    {
        Debug.Log("Player: Take Damage");
        health -= amount;

        // TODO: take type into account, and possibly even armor. 
    }
}
