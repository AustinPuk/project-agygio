using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField]
    List<Enemy> nightMonsters;

    [SerializeField]
    Transform enemyGroup;

    [SerializeField]
    Transform nightGroup;

    [SerializeField]
    private Transform sun;

    [SerializeField]
    private Transform moon;

    [SerializeField]
    private float rotationRate;

    [SerializeField]
    private float nightLength;

    [SerializeField]
    private float invadeDelay;

    [SerializeField]
    private int bossFrequency;

    public int dayNumber;

    public bool isDay;
    private float nightTimer;

    private float invadeTimer;
    private bool invadeEvent;
    private bool bossEvent;

    private Quaternion originalRotation;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }

    // Use this for initialization
    void Start () {

        originalRotation = sun.localRotation;

        isDay = true;
        sun.gameObject.SetActive(true);
        moon.gameObject.SetActive(false);

        dayNumber = 1;
    }
	
	// Update is called once per frame
	void Update () {

        if (Player.gamePause)
        {
            sun.gameObject.SetActive(true);
            moon.gameObject.SetActive(false);
            sun.localRotation = originalRotation;
            isDay = true;
            return;
        }            

        if (isDay)
        {
            sun.RotateAround(new Vector3(0.0f, 0.0f, 0.0f), Vector3.forward, rotationRate * Time.deltaTime);

            // Going from -10 degrees -> 190 degrees
            Vector3 rotation = sun.rotation.eulerAngles;
            if (rotation.x > 190.0f)
            {
                isDay = false;
                sun.gameObject.SetActive(false);
                moon.gameObject.SetActive(true);
                nightTimer = nightLength;
                sun.rotation = Quaternion.Euler(-90.0f, 0, 0);

                invadeTimer = invadeDelay;
                invadeEvent = false;
                bossEvent = false;
            }
        }
        else
        {
            CheckNight();

            if (nightTimer > 0.0f)
                nightTimer -= Time.deltaTime;
            else
            {
                isDay = true;
                sun.gameObject.SetActive(true);
                moon.gameObject.SetActive(false);
                sun.rotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
                sun.RotateAround(new Vector3(0.0f, 0.0f, 0.0f), Vector3.forward, -89.0f);

                NextDay();
            }
        }
    }

    void NextDay()
    {
        dayNumber++;

        foreach (Transform child in enemyGroup)
        {
            Enemy enemy = child.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.ChangeLevel(dayNumber);
            }
        }

        foreach (Transform child in nightGroup)
        {
            Enemy enemy = child.GetComponent<Enemy>();
            if (enemy)
            {
                enemy.TakeDamage(10000.0f, Effects.LIGHTNING);
            }
        }
    }

    void CheckNight()
    {
        if (!Player.instance.hasLight)
        {
            if (invadeTimer > 0.0f)
                invadeTimer -= Time.deltaTime;
            
            if (invadeTimer <= 0.0f && !invadeEvent)
            {
                SpawnCrawlers();
                invadeEvent = true;
            }
        }
        else
        {
            invadeTimer = invadeDelay;
        }

        if (dayNumber % bossFrequency == 0 && !bossEvent)
        {
            bossEvent = true;

            Enemy boss = Instantiate(nightMonsters[1]);
            Vector3 loc = Player.instance.transform.position + new Vector3(Random.Range(10.0f, 15.0f), 0.0f, Random.Range(10.0f, 15.0f));
            loc.y = WorldGenerator.instance.HeightLookup(loc.x, loc.z);

            boss.ChangeLevel(dayNumber);
            boss.transform.position = loc;
            boss.transform.SetParent(nightGroup);
        }
    }

    void SpawnCrawlers()
    {
        //Spawns 5 to hunt down player

        for (int i = 0; i < 5; i++)
        {
            Enemy crawler = Instantiate(nightMonsters[0]);
            Vector3 loc = Player.instance.transform.position + new Vector3(Random.Range(-10.0f, 10.0f), 0.0f, Random.Range(-10.0f, 10.0f));
            loc.y = WorldGenerator.instance.HeightLookup(loc.x, loc.z);

            crawler.ChangeLevel(dayNumber);
            crawler.transform.position = loc;
            crawler.transform.SetParent(nightGroup);
        }                
    }

    public void ResetGame()
    {
        NextDay();
        sun.gameObject.SetActive(true);
        moon.gameObject.SetActive(false);
        sun.localRotation = originalRotation;
        isDay = true;
        dayNumber = 1;
    }


}
