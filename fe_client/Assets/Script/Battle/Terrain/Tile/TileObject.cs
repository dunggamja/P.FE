using UnityEditor;
using UnityEditor.SceneManagement;


#if UNITY_EDITOR
using UnityEngine;
#endif


// 에디터 전용 코드.
[ExecuteInEditMode]
public class TileObject : MonoBehaviour
{
#if UNITY_EDITOR
    // private Terrain m_terrain = null;
    // private Vector3 m_position_last = Vector3.zero;
    // private Terrain Terrain
    // {
    //     get
    //     {
    //         if (m_terrain == null)
    //         {
    //             m_terrain = GameObject.FindGameObjectWithTag(Battle.Constants.TAG_BATTLE_TERRAIN).GetComponent<Terrain>();
    //         }

    //         return m_terrain;
    //     }
    // }

    private void SnapToTerrain(Transform _transform)
    {
        // var terrain = Terrain;
        // terrain.terrainData.size.y;

        if (_transform == null)
            return;


        var terrain_layer = 1 << LayerMask.NameToLayer(Battle.Constants.LAYER_TERRAIN);
        var origin        = _transform.position + Vector3.up * 50f;
        var direction     = Vector3.down;


        // 지형이 찾을수 없다면 0으로...
        var terrain_height = (Physics.Raycast(origin, direction, out RaycastHit hit, 100f, terrain_layer)) 
                           ? hit.point.y
                           : 0f;


        // 지형 높이에 맞춰줍시다.
        var position        = _transform.position;
        position.y          = terrain_height;
        _transform.position = position;
    }

    void OnEnable()
    {
        EditorSceneManager.sceneSaved += OnSceneSaved;
        AssemblyReloadEvents.afterAssemblyReload += OnAssemblyReload;
        EditorApplication.update += OnEditorUpdate;
    }

    void OnDisable()
    {
        EditorSceneManager.sceneSaved -= OnSceneSaved;
        AssemblyReloadEvents.afterAssemblyReload -= OnAssemblyReload;
        EditorApplication.update -= OnEditorUpdate;
    }

    void OnSceneSaved(UnityEngine.SceneManagement.Scene _scene)
    {
        if (Application.isPlaying)
            return;


        // SnapToTerrain();
        // m_position_last = transform.position;
    }

    void OnAssemblyReload()
    {
        if (Application.isPlaying)
            return;

        // SnapToTerrain();
        // m_position_last = transform.position;
    }

    // Update is called once per frame
    void OnEditorUpdate()
    {
        if (Application.isPlaying)
            return;



        for(int i = 0; i < transform.childCount; i++)
        {
            SnapToTerrain(transform.GetChild(i));
        }


        //if (m_position_last == transform.position)
        // //    return;

        // // 
        // SnapToTerrain();        

        // m_position_last = transform.position;
    }

#endif
}
