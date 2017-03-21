using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    // Simplest Enemy Ai -> Just move straight to destination

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float height;

    [SerializeField]
    private float stopDistance;

    private Vector3 destination;  

    private bool isPaused;

	// Use this for initialization
	void Start () {
        isPaused = false;
        
        float y = WorldGenerator.instance.HeightLookup(transform.position.x, transform.position.z);
        transform.position = new Vector3(transform.position.x, y + height, transform.position.z);        
    }
	
	// Update is called once per frame
	void Update () {

        if (isPaused)
            return;

        if (Vector3.Distance(transform.position, destination) <= stopDistance)
        {
            Debug.Log("Enemy: Stop");
            destination = new Vector3(0.0f, 0.0f, 0.0f);
            return;
        }

        if (Vector3.Magnitude(destination) != 0.0f)
        {
            destination = new Vector3(destination.x, WorldGenerator.instance.HeightLookup(destination.x, destination.z), destination.z);

            transform.position = transform.position + Vector3.Normalize(destination - transform.position) * moveSpeed * Time.deltaTime;
            float y = WorldGenerator.instance.HeightLookup(transform.position.x, transform.position.z);
            transform.position = new Vector3(transform.position.x, y + height, transform.position.z);
            Vector3 temp = Quaternion.FromToRotation(transform.forward, destination - transform.position).eulerAngles;
            temp.x = 0.0f;
            temp.z = 0.0f;
            transform.Rotate(temp);                           
        }
		
	}

    public void SetDestination(Vector3 dest)
    {
        destination = dest;
    }

    public void Stop()
    {
        isPaused = true;
    }

    public void Resume()
    {
        isPaused = false;
    }
}
