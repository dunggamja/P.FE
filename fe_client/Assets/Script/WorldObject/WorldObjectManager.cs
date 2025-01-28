using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public partial class WorldObjectManager : SingletonMono<WorldObjectManager>, IEventReceiver
{
    Dictionary<Int64, WorldObject> m_repository   = new(); 

    private List<WorldObject>      m_remove_queue = new(10);

    public WorldObject Seek(Int64 _id)      => m_repository.TryGetValue(_id, out var result) ? result : null;
    bool  Remove(Int64 _id)                 => m_repository.Remove(_id);
    bool  Insert(WorldObject _world_object) => (_world_object) ? m_repository.TryAdd(_world_object.ID, _world_object) : false;
    



    public void CreateObject(Int64 _id, Action<bool, Int64> _on_result = null)
    {   

        var load_asset_address = "local_base/test";
        //var load_asset_address = "test_base/test";
        //Debug.Log($"InstantiateAsync Try-1, {load_asset_address}");   
       

        AssetManager.Instance.InstantiateAsync(load_asset_address, (_object)=>
        {            
            if (_object)
            {
                var exist_object  = Seek(_id);
                if (exist_object != null)
                {
                    // 중복으로 생성이 되있을 경우 삭제 처리.
                    Debug.LogError($"already exist object, {_id}"); 
                    GameObject.Destroy(_object);
                    return;
                }


                _object.name  = $"{_id.ToString("D10")}_{load_asset_address}";
                var new_actor = _object.TryAddComponent<WorldObject>();

                new_actor.Initialize(_id);

                Insert(new_actor);

                _on_result?.Invoke(true, _id);

                //Debug.Log($"InstantiateAsync Success, {load_asset_address}");            
            }
            else
            {
                Debug.LogError($"InstantiateAsync Failed, {load_asset_address}");   

                _on_result?.Invoke(false, _id);         
            }
        });



        //Debug.Log($"InstantiateAsync Try-2, {load_asset_address}");            

        // var new_object = async_instantiate.Result;
        // if (new_object)
        // {
        //     new_object.name = $"{_id.ToString("D10")}_{load_asset_address}";
        //     var new_actor   = new_object.TryAddComponent<WorldObject>();

        //     new_actor.Initialize(_id);

        //     Insert(new_actor);
        // }
        // else
        // {
        //     Debug.Log($"InstantiateAsync Failed, {async_instantiate.Status}");            
        // }

        
        
        // var load_asset_handle  = Addressables.LoadAssetAsync<GameObject>(load_asset_address);
        // load_asset_handle.Completed += (operation) => 
        // {
        //     if (operation.Status == AsyncOperationStatus.Succeeded)
        //     {
        //         operation.
        //     }
        //     else
        //     {
        //         Debug.LogError($"load async failed, {load_asset_address}");
        //     }
        // };

        // var key = "prefab";

        // UnityEngine.ResourceManagement.ResourceLocations.
        //  Addressables.LoadAssetsAsync<GameObject>(key, (e) => 
        //  {
        //     if (e != null)
        //     {
        //         Debug.LogWarning(e.name);
        //     }
        //  });


    }


    public void DeleteObject(Int64 _id)
    {
        var world_object =  Seek(_id);
        if (world_object == null)
            return;


        Remove(_id);

        // 비활성화 후 삭제 큐에 넣어놓읍시다.
        world_object.gameObject.SetActive(false);
        m_remove_queue.Add(world_object);

        // GameObject.Destroy(world_object);
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();
        EventDispatchManager.Instance.AttachReceiver(this);
    }


    protected override void OnUpdate()
    {
        base.OnUpdate();

        // 오브젝트 삭제 처리.
        {
            foreach(var e in m_remove_queue)
            {
                GameObject.Destroy(e.gameObject);
            }
            m_remove_queue.Clear();
        }
    }

    protected override void OnRelease(bool _is_shutdown)
    {
        base.OnRelease(_is_shutdown);

        if (!_is_shutdown)
        {
            EventDispatchManager.Instance.DetachReceiver(this);
        }
    }


    // public async (bool, Actor) CreateActorAsync(Int64 _id)
    // {
    //     // TODO: Addressable 사용해보자.
    //     await;

    //     // var load_request = Resources.LoadAsync("unit/test");
    //     // await load_request.

    //     var new_object = new GameObject($"actor_{_id}");
    //     new_object.transform.localPosition = Vector3.zero;
    //     new_object.transform.localRotation = Quaternion.identity;
    //     new_object.transform.localScale    = Vector3.one;
    //     new_object.transform.SetParent(transform, false);

    //     //
    //Assets/ResourcesPrivate/unit/test.prefab


    //     return (false, null);
    // }

}