using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour {

    [SerializeField]
    private TerrainType[] possibleTypes;
    
    private List<Vector3> fixedPoints;

    private Mesh mesh;
    private TerrainType type;

    List<List<float>> heightMap;
    private float smoothness;

    public void Initialize()
    {
        fixedPoints = new List<Vector3>();

        // Get initial values
        type = possibleTypes[Random.Range(0, possibleTypes.GetLength(0))];
        foreach (Vector3 point in type.fixedPoints)
            AddFixedPoint(point);        
    }

    public void GenerateTerrain()
    {        
        int HEIGHT_MAP_SIZE = (int)Mathf.Pow(2, type.HEIGHT_MAP_POWER) + 1;
        int VILLAGE_DIAMETER = (int)(0.9f * HEIGHT_MAP_SIZE);

        if (heightMap == null)
            heightMap = new List<List<float>>();
        else
            heightMap.Clear();

        GenerateHeightMap(HEIGHT_MAP_SIZE, type.BASE_HEIGHT, VILLAGE_DIAMETER, type.HEIGHT_RANDOMNESS_SCALE, true, type.TERRAIN_SMOOTHNESS);
        GenerateMesh(type.TERRAIN_SIZE, type.TERRAIN_RESOLUTION);

        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<Renderer>().material = type.terrainMaterial;
    }

    private void GenerateMesh(float size, int numPoints)
    {
        if (mesh == null)
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        else
            mesh.Clear();
        mesh.name = "Procedural Terrain";

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
                float h1 = HeightLookup(x, z);

                float h2 = HeightLookup(x + step_size, z);
                float h3 = HeightLookup(x, z + step_size);
                float h4 = HeightLookup(x + step_size, z + step_size);

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

        /*
        if (saveFile)
        {
            AssetDatabase.CreateAsset(mesh, "Assets/Models/test.asset");
            AssetDatabase.SaveAssets();
        }
        */  
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
                
        //Initialize Four corners to some value
        heightMap[0][0] = type.BASE_HEIGHT;
        heightMap[0][size - 1] = type.BASE_HEIGHT;
        heightMap[size - 1][0] = type.BASE_HEIGHT;
        heightMap[size - 1][size - 1] = type.BASE_HEIGHT;
        
        // This part is where fixed points should be placed
        foreach (Vector3 point in fixedPoints)
            heightMap[(int)point.x][(int)point.y] = point.z;

        /*
        // Rows Test
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if ((x > 5 && x < 25) || (x > size - 40 && x < size - 10))
                    heightMap[x][y] = 50.0f;
            }
        }
        */

        /*
        //Offset of Village on Island, Round Plateau.
        float r_ratio_offset = 0.5f; //Row <----Currnently, Code only works for when village is at center of island.
        float c_ratio_offset = 0.5f; //Col
        int village_mid_r = (int) (size * r_ratio_offset);
        int village_mid_c = (int) (size * c_ratio_offset);

        int radius = village_diameter / 2;

        for (int r = village_mid_r - radius; r <= village_mid_r + radius; r++)
        {
            for (int c = village_mid_c - radius; c <= village_mid_c + radius; c++)
            {
                float dist = Vector2.Distance(new Vector2(r, c), new Vector2(village_mid_r, village_mid_c));
                //fprintf(stderr, "Dist: %f\n", dist);
                if (dist <= radius)
                {
                    heightMap[r][c] = 40.0f;
                }
            }
        }
        */

        /*
        //Makes a ramp to the plateau	
        if (ramp)
        {
            float start_slope = 1.0f;
            float end_slope = 20.0f;
            float widen_offset = 5.0f;
            unsigned int initial_widen = 5;

            for (unsigned int c = village_mid_c + radius; c < village_mid_c + radius + end_slope; c++)
            {
                float ratio = (village_mid_c + radius + end_slope - (float)c) / (end_slope - start_slope);
                unsigned int widen = (unsigned int)(widen_offset * (1.0f - ratio));
            unsigned int opening_size = initial_widen + widen;
            for (unsigned int r = village_mid_r - opening_size; r < village_mid_r + opening_size; r++)
            {
                if (c <= village_mid_c + radius + start_slope)
                {
                    height_map[r][c] = max_height;
                }
                else
                {
                    float diff = max_height - height_map[r][c];
                    height_map[r][c] = max_height - (diff * (1.f - ratio));
                }
            }
        }
        */

    //Diamond Square Algorithm

    int stepsize = size - 1;

        //Recursively creates height map
        DiamondSquare(stepsize, size, scale);
    }

    /********************** Diamond Square Method ************************/

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
    
        float r = Random.Range(0.0f, 1.0f);

        float sum = (a + b + c + d);

        heightMap[x][y] = (sum / num) + (r * scale);
        if (!type.CAN_DIP && heightMap[x][y] < 0)
            heightMap[x][y] = 0;
    }

    private void SquareStep(int x, int y, int step, int size, float scale)
    {
        if (heightMap[x][y] != -1)
            return;

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
        if (!type.CAN_DIP  && heightMap[x][y] < 0)
            heightMap[x][y] = 0;
    }
    
    /********************** Public Functions **************************/

    // x and y is in respect to the world coordinates, assuming terrain's center is at (0,0)
    // So bounds are - size / 2.0f -> size / 2.0f
    public float HeightLookup(float x, float y, float size = 0, bool atCenter = true)
    {
        float length = (size == 0) ? type.TERRAIN_SIZE : size;
        float mid = length / 2.0f;

        // Debug.Log("Height Lookup: " + x + " " + y);

        if (atCenter)
        {
            // (0, 0) is at center of terrain
            x = ((x + mid) / length) * ((float)heightMap.Count - 1.0f);
            y = ((y + mid) / length) * ((float)heightMap.Count - 1.0f);

        }
        else
        {
            // (0, 0) is at lowest corner of terrain
            x = ((x) / length) * ((float)heightMap.Count - 1.0f);
            y = ((y) / length) * ((float)heightMap.Count - 1.0f);
        }                

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

        float h0 = (heightMap[x0][y0] * (1.0f - rx)) + (heightMap[x1][y0] * rx);
        float h1 = (heightMap[x0][y1] * (1.0f - rx)) + (heightMap[x1][y1] * rx);

        return (h0 * (1.0f - ry)) + (h1 * ry);
    }

    public TerrainType GetTerrainType()
    {
        return type;
    }

    public float GetTerrainSize()
    {
        return type.TERRAIN_SIZE;
    }

    public int GetGridSize()
    {
        return (int)Mathf.Pow(2, type.HEIGHT_MAP_POWER) + 1;
    }

    public void AddFixedPoint(Vector3 vec)
    {
        fixedPoints.Add(vec);
    }

    public void AddFixedPoints(List<Vector3> points)
    {
        foreach (Vector3 point in points)            
            AddFixedPoint(point);
    }

    public void ClearFixedPoints()
    {
        fixedPoints.Clear();
    }
}
