using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyMovement))]
public class Enemy : MonoBehaviour
{
    [SerializeField]
    private string enemyType;

    [SerializeField]
    private int level;

    [Header("Combat Parameters")]

    [SerializeField]
    private float activeDist;

    [SerializeField]
    private float attackSpeed; // Attacks per second

    [SerializeField]
    private float combatRange;

    [SerializeField]
    private float sightRange;

    [SerializeField]
    private Transform patrolCenter;

    [SerializeField]
    private float patrolRange;

    [SerializeField]
    private float patrolInterval;

    [SerializeField]
    private Effects damageType;

    [SerializeField]
    private GameObject projectile;

    [SerializeField]
    private Transform projectileLoc;

    [Header("Base Stats")]

    [SerializeField]
    private int STR; // Strength 0 - 10
    [SerializeField]
    private int DEX; // Dexterity 0 - 10
    [SerializeField]
    private int CON; // Constitution 0 - 10

    public float maxHealth;
    public float health;
    public float regenRate;  // Health gained per minute
    public float baseDamage; // Only for melee

    private EnemyMovement agent;
    private Transform target;

    private float patrolTimer;
    private float attackTimer;

    void Start ()
    {
        ChangeLevel(level);
        health = maxHealth;
        patrolTimer = patrolInterval;
        attackTimer = 0.0f;

        agent = GetComponent<EnemyMovement>();

        if (!patrolCenter)
            patrolCenter = this.transform;
    }
		
	void Update ()
    {
        if (!CheckActive())
            return;

        CheckDeath();
        Regen(Time.deltaTime);

        Search();
        if (target)
            CheckAttack();
        else
            Patrol();

    }   

    /**************************** Update Functions ******************************/

    bool CheckActive()
    {
        if (Vector3.Distance(transform.position, Player.instance.transform.position) < activeDist)
            return true;
        else
            return false;
    }

    void CheckDeath()
    {
        if (health < 0.0f)
        {
            Debug.Log("Enemy Dies");
            Destroy(this.gameObject);
        }
    }

    void Search()
    {
        // Enemies have larger sight at night (Temporary. TODO: Make better system)
        float range = (WorldGenerator.instance.isDay) ? sightRange * 2 : sightRange;
        // Player already targetted, check if still in range
        if (target)
        {
            if (Vector3.Distance(target.position, transform.position) >= range)
            {
                target = null;
                patrolTimer = patrolInterval;
            }
            else
            {
                Move(target.position);
            }
        }
        else
        {
            // Adjust sight based on which angle player is from enemy's front
            float angle = Mathf.Abs(Vector3.Angle(Player.instance.transform.position - transform.position, transform.forward));
            float adjustedSight = range;
            if (angle > 45.0f && angle <= 90.0f || angle > 270.0f && angle < 315.0f)
                adjustedSight = 0.5f * range;
            else if (angle > 90.0f && angle <= 270.0f)
                adjustedSight = 0.1f * range;

            if (Vector3.Distance(Player.instance.transform.position, transform.position) < adjustedSight)
            {
                Debug.Log("Enemy: Player Found");
                target = Player.instance.transform;
                Move(target.position);
            }
        }

        // TODO: Also check for line-of-sight

    }

    void CheckAttack()
    {
        if (attackTimer > 0.0f)
        {
            attackTimer -= Time.deltaTime;
            return;
        }

        float range = (WorldGenerator.instance.isDay) ? combatRange * 2 : combatRange;

        if (Vector3.Distance(target.position, transform.position) < range)
        {
            if (!projectile) // Melee
            {
                //Debug.Log("Enemy: Attacking");
                float dmg = Random.Range(baseDamage + (-10 + (DEX * 1.5f)), baseDamage + (DEX * 1.5f)); // Dex increases accuracy of stronger attacks
                target.GetComponent<Player>().TakeDamage(dmg, Vector3.Normalize(Player.instance.transform.position - transform.position), damageType);
                Stop();
            }
            else
            {
                GameObject proj = Instantiate(projectile);
                proj.transform.position = projectileLoc.position;                
                //proj.shoot(target, );
            }

            attackTimer = (1.0f / attackSpeed);
        }
    }    

    void Patrol()
    {
        if (patrolTimer > 0.0f)
            patrolTimer -= Time.deltaTime;
        else
        {
            //Debug.Log("Enemy: Patrol New Loc");
            // Patrols random nearby locations
            Vector3 randVec = new Vector3(Random.Range(0.1f, patrolRange), 0.0f, Random.Range(0.1f, patrolRange));
            Move(patrolCenter.position + randVec);
            patrolTimer = patrolInterval;
        }       
    }

    void Regen(float deltaTime)
    {
        health = Mathf.Clamp(health + (regenRate / 60.0f) * deltaTime, 0, maxHealth);
    }


    /*************************** Helper Functions ******************************/

    void Move(Vector3 loc)
    {
        //Debug.Log("Enemy: Moving to : " + loc);        
        agent.SetDestination(loc);
        agent.Resume();
    }

    void Stop()
    {
        //Debug.Log("Enemy: Stopping");
        agent.Stop();
    }
    
    void ChangeLevel(int lvl)
    {
        level = lvl;
        maxHealth = ((8.6f * level) + 20.0f) * (CON / 7.0f);
        baseDamage = ((1.5f * level) + 8.5f) * (STR / 7.0f);
        regenRate = ((0.5f * level) + 9.5f) * (CON / 7.0f);
    }    

    IEnumerator KnockBack(Vector3 direction)
    {
        GetComponent<Rigidbody>().isKinematic = false;
        GetComponent<Rigidbody>().AddForce(direction * -500.0f);
        yield return new WaitForSeconds(0.09f);
        GetComponent<Rigidbody>().isKinematic = true;
        yield return null;
    }


    /************************** Public Functions ******************************/

    public void TakeDamage(float amount, Effects type)
    {
        //Debug.Log("Enemy Taking Damage "+ amount);
        health -= amount;
        StartCoroutine(KnockBack(Vector3.Normalize(Player.instance.transform.position - transform.position)));

        // Targets Player
    }
}
