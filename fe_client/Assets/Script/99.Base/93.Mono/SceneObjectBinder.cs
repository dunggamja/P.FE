using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SceneObjectBindingData
{
    public string       Key   = string.Empty;
    public GameObject   Value = null;
}

public class SceneObjectBinder : MonoBehaviour
{
    [SerializeField]
    Camera                       m_scene_camera    = null;


    [SerializeField]
    List<SceneObjectBindingData> m_binding_objects = new();

    public Camera SceneCamera => m_scene_camera;

    /// <summary>인스펙터에서 편집하는 바인딩 목록.</summary>
    public IReadOnlyList<SceneObjectBindingData> BindingObjects => m_binding_objects;

    public GameObject Get(string _key)
    {
        if (string.IsNullOrEmpty(_key))
            return null;

        foreach (var e in m_binding_objects)
        {
            if (e != null && e.Key == _key)
                return e.Value;
        }

        return null;
    }

    public bool TryGet(string _key, out GameObject _game_object)
    {
        _game_object = Get(_key);
        return _game_object != null;
    }


    public static SceneObjectBinder Find()
    {
        var core_object = GameObject.FindGameObjectWithTag(Battle.Constants.TAG_SCENE_CORE);
        if (core_object == null)
        {
            Debug.LogError("SceneObjectBinder: Scene Core object is not found.");
            return null;
        }

        return core_object.GetComponent<SceneObjectBinder>();
    }
}
