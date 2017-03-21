using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainType : MonoBehaviour{

    [Header("Game Objects")]
    public GameObject[] trees;
    public Vector2 treeRange;
    public GameObject[] rocks;
    public Vector2 rockRange;
    public GameObject[] items;
    public Vector2 itemRange;
    public GameObject[] enemies;
    public Vector2 enemyRange;

    [Header("Height Map Paramters")]
    public int HEIGHT_MAP_POWER = 8;
    public float BASE_HEIGHT = 10.0f;    
    public float HEIGHT_RANDOMNESS_SCALE = 50.0f;
    public bool CAN_DIP = false;

    [Header("Terrain Parameters")]
    public int TERRAIN_RESOLUTION = 30;
    public float TERRAIN_SIZE = 100.0f;
    public float TERRAIN_SMOOTHNESS = 1.2f; // Lower to make rough    
    public Material terrainMaterial;

    [Header("Fixed Points")]
    public List<Vector3> fixedPoints;
}
