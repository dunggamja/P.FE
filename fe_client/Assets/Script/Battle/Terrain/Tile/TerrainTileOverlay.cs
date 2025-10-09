using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


[ExecuteInEditMode]
public class TerrainTileOverlay : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    private Terrain m_terrain = null;

    [SerializeField]
    private Tilemap m_tilemap = null;

    [SerializeField]
    private Battle.TileData m_tile_data = null;

    [SerializeField]
    private MeshFilter m_mesh_filter = null;

    [SerializeField]
    private MeshRenderer m_mesh_renderer = null;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {

    // }

    // Update is called once per frame
    // void Update()
    // {
    // }



    void OnEnable()
    {
        EditorSceneManager.sceneSaved += OnSceneSaved;
        AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReload;
    }


    void OnDisable()
    {
        EditorSceneManager.sceneSaved -= OnSceneSaved;
        AssemblyReloadEvents.afterAssemblyReload -= OnAssemblyReload;
    }

    void OnSceneSaved(UnityEngine.SceneManagement.Scene _scene)
    {
        Debug.Log("OnSceneSaved");
        GenerateTerrainMesh();
    }

    void OnAssemblyReload()
    {
        Debug.Log("OnAssemblyReload");
        GenerateTerrainMesh();
    }

    /// <summary>
    /// 터레인 데이터를 기반으로 메시 생성
    /// </summary>
    [ContextMenu("GenerateTerrainMesh")]
    public void GenerateTerrainMesh()
    {
        // 실행 중에는 굳이...
        if (Application.isPlaying)
            return;
  
        // 메시 생성
        CreateMeshFromHeightMap();


        // 텍스쳐 생성
        RefrestTextureFromTilemap();       
    }

     /// <summary>
    /// 높이맵으로부터 메시 생성
    /// </summary>
    private void CreateMeshFromHeightMap()
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

        // var detailWidth          = terrainData.detailWidth;
        // var detailHeight         = terrainData.detailHeight;
        var heightmap_resolution = terrainData.heightmapResolution;

        // 높이맵 데이터 가져오기
        float[,] heightMap = terrainData.GetHeights(0, 0, heightmap_resolution, heightmap_resolution);


        // 텍스쳐 좌표. 월드좌표가 달라서...
        int width  = (int)terrainSize.x;
        int length = (int)terrainSize.z;

        // Debug.Log($"heightmap width: {width}, height: {height}");

        // 버텍스 배열 초기화
        var vertices  = new List<Vector3>(width * length * 4);
        var uvs       = new List<Vector2>(width * length * 4);
        var normals   = new List<Vector3>(width * length * 4);
        var triangles = new List<int>(width * length * 2 * 3 * 4);

        // 버텍스 생성
        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var vertex_index  = vertices.Count;

                var ho = ((int)heightmap_resolution / length) - 1;


                var hx = (int)(y * ((float)heightmap_resolution / length));
                var hy = (int)(x * ((float)heightmap_resolution / width));

                var pos_x = x;
                var pos_z = y;
                var pos_y_1 = heightMap[hx,      hy     ] * terrainSize.y;
                var pos_y_2 = heightMap[hx + ho, hy     ] * terrainSize.y;
                var pos_y_3 = heightMap[hx + ho, hy + ho] * terrainSize.y;
                var pos_y_4 = heightMap[hx,      hy + ho] * terrainSize.y;
                var pos_y   = Mathf.Max(pos_y_1, pos_y_2, pos_y_3, pos_y_4);

                vertices.Add(new Vector3(pos_x,     pos_y, pos_z    ) + terrainPosition);
                vertices.Add(new Vector3(pos_x + 1, pos_y, pos_z    ) + terrainPosition);
                vertices.Add(new Vector3(pos_x + 1, pos_y, pos_z + 1) + terrainPosition);
                vertices.Add(new Vector3(pos_x,     pos_y, pos_z + 1) + terrainPosition);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                uvs.Add(new Vector2((float)x       / (width - 1), (float)y       / (length - 1)));               
                uvs.Add(new Vector2((float)(x + 1) / (width - 1), (float)y       / (length - 1)));
                uvs.Add(new Vector2((float)(x + 1) / (width - 1), (float)(y + 1) / (length - 1)));
                uvs.Add(new Vector2((float)x       / (width - 1), (float)(y + 1) / (length - 1)));

                triangles.Add(vertex_index);
                triangles.Add(vertex_index + 2);
                triangles.Add(vertex_index + 1);
                triangles.Add(vertex_index);
                triangles.Add(vertex_index + 3);
                triangles.Add(vertex_index + 2);
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

        if (generated_mesh != null && m_mesh_filter != null)
        {
            m_mesh_filter.mesh = generated_mesh; 
        }
    }

    void RefrestTextureFromTilemap()
    {
        CreateTextureFromTileData();

        RefreshTextureUVFromTilemap();        
    }

    const int TILE_SIZE = 16;
    const int TILE_TEXTURE_SIZE = 256;

    (int x, int y) GetTileTextureOffset(int _tile_index)
    {
        var offset_x = (_tile_index % TILE_SIZE) * TILE_SIZE;
        var offset_y = (_tile_index / TILE_SIZE) * TILE_SIZE;

        return (offset_x, offset_y);
    }

   

    void RefreshTextureUVFromTilemap()
    {
        if (m_tilemap == null)
            return;

        if (m_terrain == null)
            return;


        var mesh           = (Application.isPlaying) ? m_mesh_filter.mesh: m_mesh_filter.sharedMesh;
        var uvs            = mesh.uv;

        var resolution     = m_terrain.terrainData.heightmapResolution;                
        var tilemap_bounds = m_tilemap.cellBounds;
        var tilemap_size   = m_terrain.terrainData.size;

        var width  = (int)tilemap_size.x;
        var length = (int)tilemap_size.z;


        for (int y = 0; y < length; y++)
        {
            for (int x = 0; x < width; x++)
            {

                var index      = (y * width + x) * 4;


                var tile       = m_tilemap.GetTile(new Vector3Int(x, y, 0));

                var tile_index = (int)m_tile_data.GetTerrainAttribute(tile);

                var (tile_ox, tile_oy) = GetTileTextureOffset(tile_index);
                
                var u    = (float)tile_ox / (TILE_TEXTURE_SIZE);
                var v    = (float)tile_oy / (TILE_TEXTURE_SIZE);
                var size = (float)TILE_SIZE / (TILE_TEXTURE_SIZE);


                uvs[index + 0] = new Vector2(u,        v);
                uvs[index + 1] = new Vector2(u + size, v);
                uvs[index + 2] = new Vector2(u + size, v + size);
                uvs[index + 3] = new Vector2(u,        v + size);
            }
        }

        mesh.uv = uvs;


    }

    void CreateTextureFromTileData()
    {
        var texture        = new Texture2D(TILE_TEXTURE_SIZE, TILE_TEXTURE_SIZE, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode   = TextureWrapMode.Clamp;

        if (m_tile_data != null)
        {
            foreach (var e in m_tile_data.m_tiles)
            {
                var tile_index = (int)e.Attribute;
                var (tile_ox, tile_oy) = GetTileTextureOffset(tile_index);

                if (e.Tile is Tile tile)
                {
                    if (tile.sprite != null)
                    {
                        var tile_texture = tile.sprite.texture;
                        var tile_rect    = tile.sprite.rect;

                        Debug.Log($"name:{tile.name} tile_rect: {tile_rect.min.x}, {tile_rect.min.y}, {tile_rect.width}, {tile_rect.height}");

                        for (int y = 0; y < TILE_SIZE; y++)
                        {
                            for (int x = 0; x < TILE_SIZE; x++)
                            {
                                var tile_offset_x = (float)x / (TILE_SIZE - 1) * tile_rect.width;
                                var tile_offset_y = (float)y / (TILE_SIZE - 1) * tile_rect.height;

                                texture.SetPixel
                                (
                                    tile_ox + x,
                                    tile_oy + y,
                                    tile_texture.GetPixel(
                                        (int)tile_rect.min.x + (int)tile_offset_x,
                                        (int)tile_rect.min.y + (int)tile_offset_y)
                                );                               

                            }
                        }
                    }                    
                }
            }
        }


        texture.Apply();


        if(m_mesh_renderer != null)
        {
            if (Application.isPlaying)
            {
                if (m_mesh_renderer.material != null)
                {
                    m_mesh_renderer.material.mainTexture = texture;
                }
            }
            else
            {
                if (m_mesh_renderer.sharedMaterial != null)
                {
                    m_mesh_renderer.sharedMaterial.mainTexture = texture;
                }
            }
        }
    }



#endif
}
