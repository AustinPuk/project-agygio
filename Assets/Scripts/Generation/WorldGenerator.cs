﻿using System.Collections;
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
        GenerateTerrain();
        GenerateTrees();
        GenerateRocks();
        GenerateItems();
        GenerateEnemies();
    }

    public float HeightLookup(float x, float z)
    {
        float size = terrains[1].GetTerrainSize();
        // Bounds Check
        if (x < (size * -1.5f) || x > (size * 1.5f) ||
            z < (size * -1.5f) || x > (size * 1.5f))
        {
            Debug.Log("Error: Height Lookup Invalid");
            return 0.0f;
        }            

        // Finds nearest terrain
        TerrainGenerator closestTerrain = terrains[1];
        float minDist = Mathf.Infinity;
        foreach (TerrainGenerator terrain in terrains)
        {
            float dist = Vector2.Distance(new Vector2(terrain.transform.position.x, terrain.transform.position.z), new Vector2(x, z));
            if (dist < minDist)
            {
                closestTerrain = terrain;
                minDist = dist;
            }                
        }

        Debug.Log("World Height: " + x + " " + z + " " + closestTerrain);
        return closestTerrain.HeightLookup(x - closestTerrain.transform.position.x, z - closestTerrain.transform.position.z);
    }

    /*************** Generation Functions *********************/

    private void GenerateTerrain()
    {
        // Initialize all terrains first
        foreach (TerrainGenerator terrain in terrains)
            terrain.Initialize();

        int gridSize = terrains[1].GetGridSize(); // All terrains should have same grid size

        // terrains[0].GenerateTerrain(); // Special, not used

        // Center
        terrains[1].GenerateTerrain();

        // Generate adjacent first, then corners to minimize overall influence
        List<Vector3> fixedPoints = new List<Vector3>();

        // Side Terrains Share Side with Center       

        // Top Center        
        fixedPoints.Clear();
        for (int i = 0; i < gridSize; i++)
            fixedPoints.Add(new Vector3(i, 0, terrains[1].HeightLookup(i, gridSize - 1, gridSize - 1, false)));
        terrains[3].AddFixedPoints(fixedPoints);
        terrains[3].GenerateTerrain();

        // Left Middle
        fixedPoints.Clear();
        for (int i = 0; i < gridSize; i++)
            fixedPoints.Add(new Vector3(gridSize - 1, i, terrains[1].HeightLookup(0, i, gridSize - 1, false)));
        terrains[5].AddFixedPoints(fixedPoints);
        terrains[5].GenerateTerrain();

        // Right Middle
        fixedPoints.Clear();
        for (int i = 0; i < gridSize; i++)
            fixedPoints.Add(new Vector3(0, i, terrains[1].HeightLookup(gridSize - 1, i, gridSize - 1, false)));
        terrains[6].AddFixedPoints(fixedPoints);
        terrains[6].GenerateTerrain();

        // Bot Center
        fixedPoints.Clear();
        for (int i = 0; i < gridSize; i++)
            fixedPoints.Add(new Vector3(i, gridSize - 1, terrains[1].HeightLookup(i, 0, gridSize - 1, false)));
        terrains[8].AddFixedPoints(fixedPoints);
        terrains[8].GenerateTerrain();

        // Corner Terrains Share Side Two Adjacent

        // Top Left
        fixedPoints.Clear();
        for (int i = 0; i < gridSize; i++)
        {
            fixedPoints.Add(new Vector3(gridSize - 1, i, terrains[3].HeightLookup(0, i, gridSize - 1, false)));
            fixedPoints.Add(new Vector3(i, 0, terrains[5].HeightLookup(i, gridSize - 1, gridSize - 1, false)));
        }
        terrains[2].AddFixedPoints(fixedPoints);
        terrains[2].GenerateTerrain();

        // Top Right
        fixedPoints.Clear();
        for (int i = 0; i < gridSize; i++)
        {
            fixedPoints.Add(new Vector3(0, i, terrains[3].HeightLookup(gridSize - 1, i, gridSize - 1, false)));
            fixedPoints.Add(new Vector3(i, 0, terrains[6].HeightLookup(i, gridSize - 1, gridSize - 1, false)));
        }
        terrains[4].AddFixedPoints(fixedPoints);
        terrains[4].GenerateTerrain();

        // Bot Left
        fixedPoints.Clear();
        for (int i = 0; i < gridSize; i++)
        {
            fixedPoints.Add(new Vector3(i, gridSize - 1, terrains[5].HeightLookup(i, 0, gridSize - 1, false)));
            fixedPoints.Add(new Vector3(gridSize - 1, i, terrains[8].HeightLookup(0, i, gridSize - 1, false)));
        }
        terrains[7].AddFixedPoints(fixedPoints);
        terrains[7].GenerateTerrain();

        // bot Right
        fixedPoints.Clear();
        for (int i = 0; i < gridSize; i++)
        {
            fixedPoints.Add(new Vector3(i, gridSize - 1, terrains[6].HeightLookup(i, 0, gridSize - 1, false)));
            fixedPoints.Add(new Vector3(0, i, terrains[8].HeightLookup(gridSize - 1, i, gridSize - 1, false)));
        }
        terrains[9].AddFixedPoints(fixedPoints);
        terrains[9].GenerateTerrain();
    }

    private void GenerateTrees()
    {        
        foreach(TerrainGenerator terrain in terrains)
        {
            TerrainType type = terrain.GetTerrainType();
            
            float middle = terrain.GetTerrainSize() / 2.0f;
            float min_x = -middle;
            float min_z = -middle;
            float max_x = middle;
            float max_z = middle;

            int numTrees = Random.Range((int) type.treeRange.x, 
                                        (int) type.treeRange.y);

            for (int i = 0; i < numTrees; i++)
            {
                GameObject newTree = Instantiate(type.trees[Random.Range(0, type.trees.Length)]);

                float x = Random.Range(min_x, max_x);
                float z = Random.Range(min_z, max_z);
                float y = terrain.HeightLookup(x, z);

                newTree.transform.position = new Vector3(x + terrain.transform.position.x, y, 
                                                         z + terrain.transform.position.z);
                newTree.transform.SetParent(terrain.transform);
            }
        }        
    }

    private void GenerateRocks()
    {
        // Similar to Generate Trees
    }

    private void GenerateItems()
    {
        // Generate random items scattered around the area.
        // Types of terrains have different probabilties for each item
    }

    private void GenerateEnemies()
    {
        // Generates random enemies. This will be more strict, a single terrain shouldn't usually have
        // multiple types of enemies. Once enemies are spawned, they take on their own behaviours.
        // Some enemies may disappear in the morning, then reappear at night.


    }
}