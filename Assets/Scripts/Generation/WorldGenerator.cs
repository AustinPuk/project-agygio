using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    public static WorldGenerator instance;

    // Handles overall world management, mainly global height lookup
    // Also delegates terrain generation so that fixed points match

    [SerializeField]
    int seed;

    [SerializeField]
    bool saveFile = false;

    [SerializeField]
    List<TerrainGenerator> terrains;

    private void Awake()
    {
        if (!instance)
            instance = this;

        if (seed != 0)
            Random.InitState(seed);

        GenerateWorld();
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
   
    /***************** Core Functions ************************/

    private void GenerateWorld()
    {
        List<Vector3> fixedPoints = new Vector3(terrains[0].getSize(0), terrains[0].ty)
        // Generate Center World        
        terrains[0].GenerateTerrain();


        // Generate Corner Terrains

    }

    public float HeightLookup(float x, float z)
    {
        // Returns the height at the given location, spanning all 9 terrains.
        return 0; 
    }

    /*************** Generation Functions *********************/

    private void GenerateTrees(float size, int numTrees, GameObject[] trees)
    {
        // TODO: Change parameters to allow offset to specific terrain are we are currently in
        float middle = size / 2.0f;
        float min_x = -middle;
        float min_z = -middle;
        float max_x = middle;
        float max_z = middle;

        for (int i = 0; i < numTrees; i++)
        {
            GameObject newTree = Instantiate(trees[Random.Range(0, trees.Length)]);

            float x = Random.Range(min_x, max_x);
            float z = Random.Range(min_z, max_z);
            float y = HeightLookup(x, z);

            newTree.transform.position = new Vector3(x, y, z);
            newTree.transform.SetParent(this.transform);
        }
    }

    private void GenerateRocks(float size, int numRocks, GameObject[] rocks)
    {
        // Similar to Generate Trees
    }

    private void GenerateItems(float size, int numItems, GameObject[] items, float[] itemChances)
    {
        // Generate random items scattered around the area.
        // Types of terrains have different probabilties for each item
    }

    private void GenerateEnemies(float size, int numEnemies, GameObject[] enemies, float[] enemyChances)
    {
        // Generates random enemies. This will be more strict, a single terrain shouldn't usually have
        // multiple types of enemies. Once enemies are spawned, they take on their own behaviours.
        // Some enemies may disappear in the morning, then reappear at night.


    }
}
