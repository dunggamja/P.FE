using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using Battle;



#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


[ExecuteInEditMode]
public class TerrainTileOverlay : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    private Terrain           m_terrain            = null;
          
    [SerializeField]          
    private Tilemap           m_tilemap            = null;

    [SerializeField]
    private Battle.TileData   m_tile_data          = null;
  
    [SerializeField]  
    private MeshFilter        m_mesh_filter        = null;
     
    [SerializeField]     
    private MeshRenderer      m_mesh_renderer      = null;
     
    [SerializeField]     
    private Transform         m_fixed_objects_root = null;
   







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
        // 실행 중에는 굳이... 할 필요 없겠지...
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
        float[,] height_map = terrainData.GetHeights(0, 0, heightmap_resolution, heightmap_resolution);


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
                var vertex_index = vertices.Count;
                var pos_x        = x;
                var pos_z        = y;      

                // 높이는 지형 & 고정 오브젝트 중 높은 값을 사용.   
                // TODO: 해놓고 보니 타일들 소팅 문제가 있다.  
                // (ztest? zorder? 쉐이더 수정하면 될거 같은데..)       
                var pos_y        = Mathf.Max(

                    // 지형 높이값
                    GetHeight_FromTerrain(
                        height_map, 
                        heightmap_resolution, 
                        terrainSize, 
                        x, y),

                    // 고정 오브젝트 높이값
                    GetHeight_FromFixedObjects(x, y));

                vertices.Add(new Vector3(pos_x,     pos_y, pos_z    ) + terrainPosition);
                vertices.Add(new Vector3(pos_x + 1, pos_y, pos_z    ) + terrainPosition);
                vertices.Add(new Vector3(pos_x + 1, pos_y, pos_z + 1) + terrainPosition);
                vertices.Add(new Vector3(pos_x,     pos_y, pos_z + 1) + terrainPosition);
                
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);
                normals.Add(Vector3.up);

                uvs.Add(new Vector2(0f, 0f));
                uvs.Add(new Vector2(0f, 0f));
                uvs.Add(new Vector2(0f, 0f));
                uvs.Add(new Vector2(0f, 0f));

                // uvs.Add(new Vector2((float)x       / (width - 1), (float)y       / (length - 1)));               
                // uvs.Add(new Vector2((float)(x + 1) / (width - 1), (float)y       / (length - 1)));
                // uvs.Add(new Vector2((float)(x + 1) / (width - 1), (float)(y + 1) / (length - 1)));
                // uvs.Add(new Vector2((float)x       / (width - 1), (float)(y + 1) / (length - 1)));

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

    float GetHeight_FromTerrain(
        float[,] _height_map,
        int      _resolution, 
        Vector3  _terrain_size, 
        int      _x, int _y)
    {
        if (_height_map == null)
            return 0f;


        // x,y 좌표가 반대임.
        int width     = (int)_terrain_size.x;
        int length    = (int)_terrain_size.z;

        int ox = ((int)_resolution / length) - 1;
        int oy = ((int)_resolution / width) - 1;
        int hx = (int)(_y * ((float)_resolution / width));
        int hy = (int)(_x * ((float)_resolution / length));


        var pos_y_1 = _height_map[hx,      hy     ] * _terrain_size.y;
        var pos_y_2 = _height_map[hx + ox, hy     ] * _terrain_size.y;
        var pos_y_3 = _height_map[hx + ox, hy + oy] * _terrain_size.y;
        var pos_y_4 = _height_map[hx,      hy + oy] * _terrain_size.y;
        var pos_y   = Mathf.Max(pos_y_1, pos_y_2, pos_y_3, pos_y_4);

        return pos_y;
    }

    float GetHeight_FromFixedObjects(int _x, int _y)
    {
        var fixed_objects_layer = 1 << LayerMask.NameToLayer(Constants.LAYER_FIXED_OBJECT);

        if (Util.RaycastToTerrain(_x, _y, out var hit, fixed_objects_layer))
        {
            if (hit.collider != null)
                return hit.collider.bounds.max.y;
        }

        return 0f;
    }



    void RefrestTextureFromTilemap()
    {
        CreateTextureFromTileData();

        RefreshTextureUV();        
    }

    const int TILE_SIZE = 16;
    const int TILE_TEXTURE_SIZE = 256;

    (int x, int y) GetTileTextureOffset(int _tile_index)
    {
        var offset_x = (_tile_index % TILE_SIZE) * TILE_SIZE;
        var offset_y = (_tile_index / TILE_SIZE) * TILE_SIZE;

        return (offset_x, offset_y);
    }

    void RefreshTextureUV()
    {
        if (m_tilemap == null)
            return;

        if (m_terrain == null)
            return;


        var mesh           = (Application.isPlaying) ? m_mesh_filter.mesh: m_mesh_filter.sharedMesh;
        var uvs            = mesh.uv;

        // var resolution     = m_terrain.terrainData.heightmapResolution;                
        // var tilemap_bounds = m_tilemap.cellBounds;
        var tilemap_size   = m_terrain.terrainData.size;

        var width     = (int)tilemap_size.x;
        var length    = (int)tilemap_size.z;

        var tile_data = new int[width, length];
        Array.Clear(tile_data, 0, tile_data.Length);
        
        

        CollectTileData_Tilemap(ref tile_data, width, length);

        CollectTileData_FixedObjects(ref tile_data, width, length);

        RefreshTextureUVFromTileData(ref tile_data, ref uvs, width, length);

        mesh.uv = uvs;
    }




   

    void CollectTileData_Tilemap(ref int[,] _tile_data, int _width, int _length)
    {
        if (_tile_data == null)
            return;


        for (int y = 0; y < _length; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                var tile           = m_tilemap.GetTile(new Vector3Int(x, y, 0));
                var tile_attribute = m_tile_data.GetTerrainAttribute(tile);

                _tile_data[x, y] |= 1 << (int)tile_attribute;
            }
        }
    }

    void CollectTileData_FixedObjects(ref int[,] _tile_data, int _width, int _length)
    {
       if (_tile_data == null)
            return;       

        var fixed_objects = (m_fixed_objects_root) 
                          ? m_fixed_objects_root.GetComponentsInChildren<FixedObjects>()
                          : null;

        if (fixed_objects != null)
        {
            foreach (var e in fixed_objects)
            {
                if (e == null)
                    continue;

                foreach(var tile_data in e.GetTileAttributes())
                {
                    if (tile_data.x < 0 || tile_data.x >= _width 
                     || tile_data.y < 0 || tile_data.y >= _length)
                        continue;

                    _tile_data[tile_data.x, tile_data.y] |= 1 << (int)tile_data.attribute;
                }
            }
        }

    }

    void RefreshTextureUVFromTileData(ref int[,] _tile_data, ref Vector2[] _uvs, int _width, int _length)
    {
        if (_tile_data == null)
            return;

        

        // 이동불가 < 공중만 가능 < 물 < 지형 순으로 타일을 표시해봅세.
        EnumTerrainAttribute PickTileAttribute(int _tile_attribute)
        {
            Span<EnumTerrainAttribute> tile_sort_order = stackalloc EnumTerrainAttribute[] 
            { 
                EnumTerrainAttribute.Invalid, 
                EnumTerrainAttribute.FlyerOnly, 
                EnumTerrainAttribute.Water, 
                EnumTerrainAttribute.Water_Shallow,
                EnumTerrainAttribute.Ground_Climb,
                EnumTerrainAttribute.Ground_Forest,
                EnumTerrainAttribute.Ground_Dirt,
                EnumTerrainAttribute.Ground
            };

            for (int i = 0; i < tile_sort_order.Length; i++)
            {
                if ((_tile_attribute & (1 << (int)tile_sort_order[i])) != 0)
                    return tile_sort_order[i];
            }

            return EnumTerrainAttribute.Invalid;
        }
        
        
        

        for (int y = 0; y < _length; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                var tile               = _tile_data[x, y];
                var tile_attribute     = PickTileAttribute(_tile_data[x, y]);
                var (tile_ox, tile_oy) = GetTileTextureOffset((int)tile_attribute);
                
                var u           = (float)tile_ox   / (TILE_TEXTURE_SIZE);
                var v           = (float)tile_oy   / (TILE_TEXTURE_SIZE);
                var size        = (float)TILE_SIZE / (TILE_TEXTURE_SIZE);

                var index       = (y * _width + x) * 4;                
                _uvs[index + 0] = new Vector2(u,        v);
                _uvs[index + 1] = new Vector2(u + size, v);
                _uvs[index + 2] = new Vector2(u + size, v + size);
                _uvs[index + 3] = new Vector2(u,        v + size);
            }
        }
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
