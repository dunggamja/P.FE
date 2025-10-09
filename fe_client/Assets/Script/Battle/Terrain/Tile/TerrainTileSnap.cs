using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


// 에디터 전용 코드.
[ExecuteInEditMode]
public class TerrainTileSnap : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField]
    private Transform[] m_tile_objects_root = null;


    private void SnapToTerrain(Transform _transform)
    {
        // var terrain = Terrain;
        // terrain.terrainData.size.y;

        if (_transform == null)
            return;

        // if (_transform.TryGetComponent<TileObject>(out var tile_object) == false)
        //     return;

        // if (tile_object.SnapToTerrain.is_snap == false)
        //     return;


        var terrain_layer = 1 << LayerMask.NameToLayer(Battle.Constants.LAYER_TERRAIN);
        var origin        = _transform.position + Vector3.up * 100f;
        var direction     = Vector3.down;


        // 지형이 찾을수 없다면 0으로...
        var terrain_height = (Physics.Raycast(origin, direction, out RaycastHit hit, 200f, terrain_layer)) 
                           ? hit.point.y
                           : 0f;


        // 지형 높이에 맞춰줍시다.
        var position             =  _transform.position;
        position.y               =  terrain_height;
        _transform.position      =  position;
        // _transform.localPosition += tile_object.SnapToTerrain.offset;
    }

    void OnEnable()
    {
        // EditorSceneManager.sceneSaved            += OnSceneSaved;
        // AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReload;
        EditorApplication.update                 += OnEditorUpdate;
    }

    void OnDisable()
    {
        // EditorSceneManager.sceneSaved            -= OnSceneSaved;
        // AssemblyReloadEvents.afterAssemblyReload -= OnAssemblyReload;
        EditorApplication.update                 -= OnEditorUpdate;
    }

    // void OnSceneSaved(UnityEngine.SceneManagement.Scene _scene)
    // {
    //     if (Application.isPlaying)
    //         return;
    //     // SnapToTerrain();
    //     // m_position_last = transform.position;
    // }

    // void OnAssemblyReload()
    // {
    //     if (Application.isPlaying)
    //         return;

    //     // SnapToTerrain();
    //     // m_position_last = transform.position;
    // }

    // Update is called once per frame
    void OnEditorUpdate()
    {
        if (Application.isPlaying)
            return;

        //if (m_tile_objects_root != null)
        {
            //foreach(var root in m_tile_objects_root)
            {
                // if (root == null)
                //     continue;

                for(int i = 0; i < transform.childCount; i++)
                {
                    SnapToTerrain(transform.GetChild(i));
                }
            }
        }

        // GetComponentsInChildren<TileObject>((tile_object) =>
        // {
        //     SnapToTerrain(tile_object.transform);
        // });

        


        //if (m_position_last == transform.position)
        // //    return;

        // // 
        // SnapToTerrain();        

        // m_position_last = transform.position;
    }

#endif
}
