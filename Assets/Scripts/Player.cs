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

    private float health;
    private float hunger;

    void Awake()
    {
        if (!instance)
            instance = this;
    }

	// Use this for initialization
	void Start () {        		
	}
	
	// Update is called once per frame
	void Update () {

        Regen(Time.deltaTime);
		        
	}

    /**************************** Player Updates ***********************************/

    void Regen(float deltaTime)
    {
        hunger -= (hungerRate / 60.0f);
        //health += hunger * some logarithmic modifier * baseHealthRegen;
    }

    void CheckDeath()
    {
        if (health <= 0.0f)
            OnDeath();        
    }

    /***************************** Player Event Functions *************************/
    void OnDeath()
    {
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
        health -= amount;

        // TODO: take type into account, and possibly even armor. 
    }
}
