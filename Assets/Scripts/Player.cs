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

    [SerializeField]
    private GameObject fadeScreen;

    [Tooltip("Rate that hunger reduces per MINUTE")]
    [SerializeField]
    private float hungerRate;

    [Tooltip("Rate that health restores over time. Actual rate changes proportional to hunger")]
    [SerializeField]
    private float baseHealthRegen;

    [SerializeField]
    private Transform respawnLoc;

    public bool hasLight;
                
    static public bool gamePause;
    static public bool canMove;

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

        gamePause = true;
        canMove = true;
	}
	
	// Update is called once per frame
	void Update () {

        if (!gamePause)
        {
            CheckLight();
            Regen(Time.deltaTime);
            CheckDeath();
        }        		        
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

    void CheckLight()
    {
        hasLight = false;

        if (VRControls.instance.rightHand.getHeldItem())
        {
            if (VRControls.instance.rightHand.getHeldItem().itemName == "Torch")
                hasLight = true;
        }
        if (VRControls.instance.leftHand.getHeldItem())
        {
            if (VRControls.instance.leftHand.getHeldItem().itemName == "Torch")
                hasLight = true;
        }
        if (Campfire.instance.isLit)
        {
            if (Vector3.Distance(transform.position, Campfire.instance.transform.position) < 10.0f)
                hasLight = true;
        }
    }

    /***************************** Player Event Functions *************************/
    void OnDeath()
    {
        //Debug.Log("Player Dies");
        // Dies
        // Darkness, then return to main menu or show score

        StartCoroutine(FadeOut(2.0f, true));

        // Set Pause Boolean for game
        Backpack.instance.Clear();       
        gamePause = true;
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
    
    IEnumerator FadeOut(float time, bool toHaven)
    {
        canMove = false;
        fadeScreen.SetActive(true);
        Material mat = fadeScreen.GetComponent<Renderer>().material;
        Color color = mat.color;

        for (float i = 0; i <= 100.0f; i++) 
        {
            color.a = i / 100.0f;
            mat.color = color;
            yield return new WaitForSeconds(time / 100.0f);   
        }
        if (toHaven)
            Teleport(respawnLoc.position, false);
        else
            Teleport(new Vector3(0, 0, 0), true);

        canMove = true;
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(FadeIn(time));

        yield return null;
    }    

    IEnumerator FadeIn(float time)
    {
        fadeScreen.SetActive(true);
        Material mat = fadeScreen.GetComponent<Renderer>().material;
        Color color = mat.color;

        for (float i = 100.0f; i >= 0; i--)
        {
            color.a = i / 100.0f;
            mat.color = color;
            yield return new WaitForSeconds(time / 100.0f);
        }

        fadeScreen.SetActive(false);
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
         
    }        

    public void StartGame()
    {
        GameManager.instance.ResetGame();
        gamePause = false;
        health = maxHealth;
        hunger = maxHunger;
        StartCoroutine(FadeOut(1.0f, false));
    }

    /******************************* Helper Functions **********************************/

    private void FixHeight() 
    {
        float playerHeight = 1.3f; // TODO: Make this variable better
        float x = transform.position.x;
        float z = transform.position.z;
        transform.position = new Vector3(x, WorldGenerator.instance.HeightLookup(x, z) + playerHeight, z);
    }

    private void Teleport(Vector3 loc, bool fixHeight)
    {
        transform.position = loc;
        if (fixHeight)
        {
            FixHeight();
        }

    }
}
