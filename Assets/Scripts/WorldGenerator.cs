using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WorldGenerator : MonoBehaviour {

    [SerializeField]
    private GameObject tree;

    private Mesh mesh;

    List<List<float>> heightMap;
    private float smoothness;

    [SerializeField]
    int HEIGHT_MAP_POWER = 8;
    [SerializeField]
    int TERRAIN_RESOLUTION = 50;
    [SerializeField]
    float TERRAIN_SIZE = 100.0f;
    [SerializeField]
    float TERRAIN_SCALE = 500.0f;    
    [SerializeField]
    float HEIGHT_MAP_MAX = 10.0f;
    [SerializeField]
    float HEIGHT_RANDOMNESS_SCALE = 100.0f;
    [SerializeField]
    float TERRAIN_SMOOTHNESS = 1.2f; // Lower to make rough


    private void Awake()
    {
        int HEIGHT_MAP_SIZE = (int)Mathf.Pow(2, HEIGHT_MAP_POWER) + 1;
        int VILLAGE_DIAMETER = (int)(0.23f * HEIGHT_MAP_SIZE);

        heightMap = new List<List<float>>();

        GenerateHeightMap(HEIGHT_MAP_SIZE, HEIGHT_MAP_MAX, VILLAGE_DIAMETER, HEIGHT_RANDOMNESS_SCALE, true, TERRAIN_SMOOTHNESS);
        GenerateTerrain(TERRAIN_SIZE, TERRAIN_RESOLUTION, 0.0f, Mathf.Infinity);
        GenerateTrees(TERRAIN_SIZE, 100);
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void GenerateTrees(float size, int numTrees)
    {
        float middle = size / 2.0f;
        float min_x = -middle;
        float min_z = -middle;
        float max_x = middle;
        float max_z = middle;

        for (int i = 0; i < numTrees; i++)
        {
            GameObject newTree = Instantiate(tree);

            float x = Random.Range(min_x, max_x);
            float z = Random.Range(min_z, max_z);
            float y = HeightLookup(x, z, size);

            newTree.transform.position = new Vector3(x, y, z);
            newTree.transform.SetParent(this.transform);
        }
    }

    private void GenerateTerrain(float size, int numPoints, float minHeight, float maxHeight)
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Terrain";

        //Experimenting. Assumed that height map is already set up and size is same as height map size

        //Terain is size x size

        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> indices = new List<int>();

        bool normals_up = false;    

        //Create array of points using height map lookup function
        //Grid is same size as height map, and scales with height map size
        float middle = size / 2.0f;
        float min_x = -middle;
        float min_z = -middle;
        float max_x = middle;
        float max_z = middle;

        float step_size = size / (float)numPoints;

        //Goes Square by Square
        for (float x = min_x; x <= (max_x - step_size); x += step_size)
        {
            for (float z = min_z; z <= (max_z - step_size); z += step_size)
            {
                float h1 = HeightLookup(x, z, size);

                if (h1 < minHeight || h1 > maxHeight)
                    continue;

                float h2 = HeightLookup(x + step_size, z, size);
                float h3 = HeightLookup(x, z + step_size, size);
                float h4 = HeightLookup(x + step_size, z + step_size, size);

                Vector3 v1 = new Vector3(x, h1, z);                         //Upper Left
                Vector3 v2 = new Vector3(x + step_size, h2, z);             //Upper Right
                Vector3 v3 = new Vector3(x, h3, z + step_size);              //Lower Left
                Vector3 v4 = new Vector3(x + step_size, h4, z + step_size);  //Lower Right

                //Make Two Triangles for Square
                vertices.Add(v1);
                vertices.Add(v3);
                vertices.Add(v2);
                vertices.Add(v2);
                vertices.Add(v3);
                vertices.Add(v4);

                if (normals_up)
                {
                    normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
                    normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
                    normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
                    normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
                    normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
                    normals.Add(new Vector3(0.0f, 1.0f, 0.0f));
                }
                else
                {
                    //Get Normals for each vertex pushed (Right hand rule ftw)
                    Vector3 n1 = Vector3.Cross(v2 - v4, v1 - v4);
                    Vector3 n2 = Vector3.Cross(v1 - v2, v4 - v2);
                    Vector3 n3 = Vector3.Cross(v4 - v1, v2 - v1);
                    Vector3 n4 = Vector3.Cross(v3 - v1, v4 - v1);
                    Vector3 n5 = Vector3.Cross(v4 - v3, v1 - v3);
                    Vector3 n6 = Vector3.Cross(v1 - v4, v3 - v4);

                    normals.Add(n1);
                    normals.Add(n2);
                    normals.Add(n3);
                    normals.Add(n4);
                    normals.Add(n5);
                    normals.Add(n6);
                }
            }
        }

        //Indices are just in order to make it easier
        for (int i = 0; i < vertices.Count; i++)
            indices.Add(i);

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = indices.ToArray();

        AssetDatabase.CreateAsset(mesh, "Assets/Models/test.asset");
        AssetDatabase.SaveAssets();

    }

    private void GenerateHeightMap(int size, float max_height, int village_diameter, float scale, bool ramp, float smooth_value)
    {        
        heightMap.Clear();

        smoothness = smooth_value;        

        //All points initialized at -1
        for (int i = 0; i < size; i++)
        {
            heightMap.Add(new List<float>());
            for (int j = 0; j < size; j++)
            {
                heightMap[i].Add(-1.0f);
            }
        }

        //fprintf(stderr, "Height Map Size: %d\t%d\t%d\n", size, height_map.size(), height_map[1].size());
        //fprintf(stderr, "Test Value: %f\n", height_map[0][0]);
        //fprintf(stderr, "Height Map Size: %d\t%f\n", test.size(), test[0]);

        //Initialize Four corners to be ground level
        heightMap[0][0] = max_height / 2.0f;
        heightMap[0][size - 1] = max_height / 2.0f;
        heightMap[size - 1][0] = max_height / 2.0f;
        heightMap[size - 1][size - 1] = max_height / 2.0f;

        /*
        //Initialize all sides ot be ground level
        for (int i = 0; i < size; i++)
        {
            heightMap[0][i] = 0;
            heightMap[size - 1][i] = 0;
            heightMap[i][0] = 0;
            heightMap[i][size - 1] = 0;
        }
        */

        //Diamond Square Algorithm

        int stepsize = size - 1;

        //Recursively creates height map
        DiamondSquare(stepsize, size, scale);
    }

    void DiamondSquare(int step, int size, float scale)
    {
        if (step <= 1)
            return;

        int halfstep = step / 2;

        for (int x = halfstep; x < size; x += step)
        {
            for (int y = halfstep; y < size; y += step)
            {
                DiamondStep(x, y, step, size, Random.Range(-1.0f, 1.0f) * scale);
            }
        }

        for (int x = 0; x < size; x += step)
        {
            for (int y = 0; y < size; y += step)
            {
                if (x + halfstep < size)
                    SquareStep(x + halfstep, y, step, size, Random.Range(-1.0f, 1.0f) * scale);
                if (y + halfstep < size)
                    SquareStep(x, y + halfstep, step, size, Random.Range(-1.0f, 1.0f) * scale);
            }
        }

        scale *= (float)Mathf.Pow(2.0f, -smoothness);

        DiamondSquare(step / 2, size, scale);
    }

    void DiamondStep(int x, int y, int step, int size, float scale)
    {
        if (heightMap[x][y] != -1)
            return;

        //fprintf(stderr, "Diamond on point %u, %u\n", x, y, size);

        int halfstep = step / 2;

        // a     b 
        //
        //    x
        //
        // c     d

        float a, b, c, d;
        float num = 4.0f;

        if (x >= halfstep && y >= halfstep)
            a = heightMap[x - halfstep][y - halfstep];
        else
            a = 0;
        if (x + halfstep < size && y >= halfstep)
            b = heightMap[x + halfstep][y - halfstep];
        else
            b = 0;
        if (x >= halfstep && y + halfstep < size)
            c = heightMap[x - halfstep][y + halfstep];
        else
            c = 0;
        if (x + halfstep < size && y + halfstep < size)
            d = heightMap[x + halfstep][y + halfstep];
        else
            d = 0;

        //float r = (float)(rand() % 101) / 100.f;        
        float r = Random.Range(0.0f, 1.0f);
        //fprintf(stderr, "Random: %f\n", r);

        //fprintf(stderr, "Using: %.2f, %.2f, %.2f, %.2f and %.2f\n", a, b, c, d, num);

        float sum = (a + b + c + d);

        heightMap[x][y] = (sum / num) + (r * scale);
        if (heightMap[x][y] < 0)
            heightMap[x][y] = 0;
    }

    private void SquareStep(int x, int y, int step, int size, float scale)
    {
        if (heightMap[x][y] != -1)
            return;

        //fprintf(stderr, "Square on point %u, %u\n", x, y);

        int halfstep = step / 2;

        //   c
        //
        //a  x  b
        //
        //   d

        float a, b, c, d;
        float num = 4.0f;

        if (x >= halfstep)
            a = heightMap[x - halfstep][y];
        else
            a = 0;
        if (x + halfstep < size)
            b = heightMap[x + halfstep][y];
        else
            b = 0;
        if (y >= halfstep)
            c = heightMap[x][y - halfstep];
        else
            c = 0;
        if (y + halfstep < size)
            d = heightMap[x][y + halfstep];
        else
            d = 0;

        if (x == 0 || y == 0 || x == size - 1 || y == size - 1)
            num = 3.0f;
        
        float r = Random.Range(0.0f, 1.0f);

        float sum = (a + b + c + d);

        heightMap[x][y] = (sum / num) + (r * scale);
        if (heightMap[x][y] < 0)
            heightMap[x][y] = 0;
    }

    private float HeightLookup(float x, float y, float length)
    {
        float mid = length / 2.0f;
        x = ((x + mid) / length) * ((float)heightMap.Count - 1.0f);
        y = ((y + mid) / length) * ((float)heightMap.Count - 1.0f);

        int x0 = (int)Mathf.Floor(x);
        int x1 = (int)Mathf.Ceil(x);
        int y0 = (int)Mathf.Floor(y);
        int y1 = (int)Mathf.Ceil(y);

        float rx, ry; //Ratios

        //Bilinear Interpolation
        if (x1 != x0)
            rx = (x - x0) / (x1 - x0);
        else
            rx = 0.0f;
        if (y1 != y0)
            ry = (y - y0) / (y1 - y0);
        else
            ry = 0.0f;

        //fprintf(stderr, "Test: %d, %d, %d, %d, %f, %f\n", x0, x1, y0, y1, x, y);

        float h0 = (heightMap[x0][y0] * (1.0f - rx)) + (heightMap[x1][y0] * rx);
        float h1 = (heightMap[x0][y1] * (1.0f - rx)) + (heightMap[x1][y1] * rx);

        return (h0 * (1.0f - ry)) + (h1 * ry);
    }
}
