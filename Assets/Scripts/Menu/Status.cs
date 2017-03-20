using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Status : MonoBehaviour {

    [SerializeField]
    private Image healthBar;

    [SerializeField]
    private Text healthText;

    [SerializeField]
    private Image hungerBar;

    [SerializeField]
    private Text hungerText;
    
    void Start ()
    {
		
	}
	
	void Update ()
    {
        healthText.text = Mathf.CeilToInt(Player.instance.health).ToString();
        healthBar.fillAmount = Player.instance.health / Player.instance.maxHealth;
        hungerText.text = Mathf.CeilToInt(Player.instance.hunger).ToString();
        hungerBar.fillAmount = Player.instance.hunger / Player.instance.maxHunger;
    }
}
