using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

	// Use this for initialization
	void Start () {        
	}
	
	// Update is called once per frame
	void Update () {
        //NavMeshAgent agent = GetComponent<NavMeshAgent>();
        //agent.destination = Player.instance.gameObject.transform.position;

    }

    public void TakeDamage(float amount, Effects type)
    {
        Debug.Log("Taking Damage");
        Destroy(this.gameObject);
    }
}
