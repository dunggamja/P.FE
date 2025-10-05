using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class TerrainTileOverlay : MonoBehaviour
{

    [SerializeField]
    private Terrain m_terrain = null;

    [SerializeField]
    private Tilemap m_tilemap = null;

    [SerializeField]
    private MeshFilter m_mesh_filter = null;


    // [SerializeField]
    // private MeshRenderer m_mesh_renderer = null;
    // [SerializeField]
    // private MeshFilter m_mesh_filter = null;




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    /// <summary>
    /// 터레인 데이터를 기반으로 메시 생성
    /// </summary>
    [ContextMenu("GenerateTerrainMesh")]
    public void GenerateTerrainMesh()
    {
        if (m_terrain == null)
        {
            Debug.LogError("Terrain is not assigned!");
            return;
        }

        // 터레인 데이터 가져오기
        TerrainData terrainData     = m_terrain.terrainData;
        Vector3     terrainSize     = terrainData.size;
        Vector3     terrainPosition = m_terrain.transform.position;

        var detailWidth          = terrainData.detailWidth;
        var detailHeight         = terrainData.detailHeight;
        var heightmap_resolution = terrainData.heightmapResolution;

        

        Debug.Log($"detailWidth: {detailWidth}, detailHeight: {detailHeight}, heightmap_resolution: {heightmap_resolution}");
        Debug.Log($"terrainSize: {terrainSize}, terrainPosition: {terrainPosition}");

        // 높이맵 데이터 가져오기
        float[,] heightMap = terrainData.GetHeights(0, 0, heightmap_resolution, heightmap_resolution);


        Debug.Log($"heightMap: {heightMap.GetLength(0)}, {heightMap.GetLength(1)}");

        // 메시 생성
        var mesh = CreateMeshFromHeightMap(heightMap, terrainSize, terrainPosition);

        if (mesh != null && m_mesh_filter != null)
        {
            m_mesh_filter.mesh = mesh; 
        }

        // // 메시 필터에 할당
        // if (m_mesh_filter != null)
        // {
        //     m_mesh_filter.mesh = m_generated_mesh;
        // }

        // // 머티리얼 할당
        // if (m_mesh_renderer != null && m_terrain_material != null)
        // {
        //     m_mesh_renderer.material = m_terrain_material;
        // }

        // Debug.Log($"Terrain mesh generated with {m_vertices.Length} vertices and {m_triangles.Length / 3} triangles");
    }

     /// <summary>
    /// 높이맵으로부터 메시 생성
    /// </summary>
    private Mesh CreateMeshFromHeightMap(float[,] heightMap, Vector3 terrainSize, Vector3 terrainPosition)
    {
        int width  = heightMap.GetLength(1);
        int height = heightMap.GetLength(0);

        Debug.Log($"heightmap width: {width}, height: {height}");

        // 버텍스 배열 초기화
        var vertices  = new List<Vector3>(width * height);
        var uvs       = new List<Vector2>(width * height);
        var normals   = new List<Vector3>(width * height);
        var triangles = new List<int>(width * height * 2 * 3);


        // var heightmap_multiplier = 1f;

        // var max_zpos = 0f;
        // var max_xpos = 0f;

        // width = 4;
        // height = 4;


        var x_step = terrainSize.x / (width - 1);
        var z_step = terrainSize.z / (height - 1);


        // 버텍스 생성
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var pos_x = (float)x / (width - 1) * terrainSize.x;
                var pos_z = (float)y / (height - 1) * terrainSize.z;
                var pos_y = heightMap[y, x] * terrainSize.y;

                vertices.Add(new Vector3(pos_x, pos_y, pos_z) + terrainPosition);
                
                normals.Add(Vector3.up);


                uvs.Add(new Vector2((float)x / (width - 1), (float)y / (height - 1)));
               
            }
        }

        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                int index = y * width + x;

                triangles.Add(index);
                triangles.Add(index + width);
                triangles.Add(index + width + 1);
                triangles.Add(index);
                triangles.Add(index + width + 1);
                triangles.Add(index + 1);
            }
        }

        // 메시 생성
        var generated_mesh         = new Mesh();
        generated_mesh.name        = "Generated Terrain Mesh";
        generated_mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        generated_mesh.vertices    = vertices.ToArray();
        generated_mesh.triangles   = triangles.ToArray();
        generated_mesh.uv          = uvs.ToArray();
        generated_mesh.normals     = normals.ToArray();
        generated_mesh.RecalculateBounds();


        

        return generated_mesh;
    }


        /// <summary>
    /// 노멀 벡터 계산
    /// </summary>
    private void CalculateNormals(int width, int height, Vector3[] vertices, Vector3[] normals)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                normals[index] = Vector3.up;
                // Vector3 normal = Vector3.zero;

                // // 주변 버텍스들과의 차이를 이용해 노멀 계산
                // if (x > 0 && x < width - 1 && y > 0 && y < height - 1)
                // {
                //     Vector3 left = vertices[index - 1];
                //     Vector3 right = vertices[index + 1];
                //     Vector3 up = vertices[index - width];
                //     Vector3 down = vertices[index + width];

                //     Vector3 horizontal = right - left;
                //     Vector3 vertical = down - up;

                //     normal = Vector3.Cross(horizontal, vertical).normalized;
                // }
                // else
                // {
                //     normal = Vector3.up; // 경계에서는 위쪽 노멀 사용
                // }

                // normals[index] = normal;
            }
        }
    }

    /// <summary>
    /// 트라이앵글 인덱스 생성
    /// </summary>
    private List<int> GenerateTriangles(int width, int height)
    {
        List<int> triangles = new List<int>();

        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                int index_bl = y * width + x;
                int index_br = index_bl  + 1;

                int index_tl = index_bl  + width;
                int index_tr = index_tl  + 1;


                // 첫 번째 트라이앵글 (시계방향)
                triangles.Add(index_bl);
                triangles.Add(index_tl);
                triangles.Add(index_br);

                // 두 번째 트라이앵글 (시계방향)
                triangles.Add(index_br);
                triangles.Add(index_bl);
                triangles.Add(index_tr);
            }
        }

        return triangles;
    }


}
