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


    [SerializeField]
    private MeshRenderer m_mesh_renderer = null;


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
        CreateMeshFromHeightMap(heightMap, terrainSize, terrainPosition);


        // 텍스쳐 생성
        CreateTextureFromTilemap();
        

       
    }

     /// <summary>
    /// 높이맵으로부터 메시 생성
    /// </summary>
    private void CreateMeshFromHeightMap(float[,] heightMap, Vector3 terrainSize, Vector3 terrainPosition)
    {
        int width  = heightMap.GetLength(1);
        int height = heightMap.GetLength(0);

        // Debug.Log($"heightmap width: {width}, height: {height}");

        // 버텍스 배열 초기화
        var vertices  = new List<Vector3>(width * height);
        var uvs       = new List<Vector2>(width * height);
        var normals   = new List<Vector3>(width * height);
        var triangles = new List<int>(width * height * 2 * 3);

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

        if (generated_mesh != null && m_mesh_filter != null)
        {
            m_mesh_filter.mesh = generated_mesh; 
        }
    }

    void CreateTextureFromTilemap()
    {
        if (m_tilemap == null)
            return;

        const int TILE_SIZE = 16;
        
        var texture        = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode   = TextureWrapMode.Clamp;





        var tiles = new HashSet<TileBase>();
        var tilemap_bounds = m_tilemap.cellBounds;
        for (int z = tilemap_bounds.zMin; z < tilemap_bounds.zMax; z++)
        {
            for (int x = tilemap_bounds.xMin; x < tilemap_bounds.xMax; x++)
            {
                var tile = m_tilemap.GetTile(new Vector3Int(x, 0, z));
                if (tile != null && !tiles.Contains(tile))
                    tiles.Add(tile);
            }
        }

            // 모든 타일을 순회하며 특정 타입만 삭제
        for (int y = tilemap_bounds.yMin; y < tilemap_bounds.yMax; y++)
        {
            for (int x = tilemap_bounds.xMin; x < tilemap_bounds.xMax; x++)
            {
                for (int z = tilemap_bounds.zMin; z < tilemap_bounds.zMax; z++)
                {
                    Vector3Int cellPos = new Vector3Int(x, y, z);
                    TileBase tile = m_tilemap.GetTile(cellPos);

                    Debug.Log($"tile: {tile}, cellPos: {cellPos}");    
                }
            }
        }
        




        Debug.Log($"tiles: {tiles.Count}");
        Debug.Log($"tilemap_bounds: {tilemap_bounds}");


        var tile_per_row = texture.width / TILE_SIZE;
        var tile_index   = 0;
        

        foreach (var tile in tiles)
        {
            var rule_tile = tile as RuleTile;
            if (rule_tile == null || rule_tile.m_DefaultSprite == null)
                continue;

            var tile_sprite = rule_tile.m_DefaultSprite;
            if (tile_sprite == null || tile_sprite.texture == null)
                continue;

            var tile_texture = tile_sprite.texture;
            var tile_rect    = tile_sprite.rect;

            var texture_x = (tile_index % tile_per_row) * TILE_SIZE;
            var texture_y = (tile_index / tile_per_row) * TILE_SIZE;

            for (int y = 0; y < tile_rect.height; y++)
            {
                for (int x = 0; x < tile_rect.width; x++)
                {
                    var u = tile_rect.x + x / (float)TILE_SIZE * tile_rect.width;
                    var v = tile_rect.y + y / (float)TILE_SIZE * tile_rect.height;

                    texture.SetPixel(texture_x + x, texture_y + y, tile_texture.GetPixel(x, y));
                }
            }

            tile_index++;            
        }




        


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


        texture.Apply();
    }




}
