using System;
using System.Collections.Generic;
using Battle;
using Cysharp.Threading.Tasks;
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
    



    public async void CreateObject(Int64 _id)//, Action<bool, Int64> _on_result = null)
    {   
        //async
        var load_asset_address = AssetName.TEST_PREFAB;
        //var load_asset_address = "test_base/test";
        //Debug.Log($"InstantiateAsync Try-1, {load_asset_address}");   
        
        // unitask 대기.
        var new_object = await AssetManager.Instance.InstantiateAsync(load_asset_address);

        // 대기 완료 후 처리.
        if (new_object != null)
        {
            var entity            = EntityManager.Instance.GetEntity(_id);
            var duplicate_object  = Seek(_id);

            // 중복 오브젝트 생성 or entity가 없으면 생성 실패 처리.
            var is_error = (duplicate_object != null) || (entity == null);
            if (is_error)
            {
                Debug.LogError($"error, create object, {_id}"); 
                GameObject.Destroy(new_object);
                return;
            }

            new_object.name = $"{_id.ToString("D10")}_{load_asset_address}";
            var new_actor   = new_object.TryAddComponent<WorldObject>();

            new_actor.Initialize(entity);

            Insert(new_actor);

            // 생성했을 때 이벤트 처리.

            // Debug.Log($"InstantiateAsync Success, {_id}");  
        }   
        else
        {
            Debug.Log($"InstantiateAsync Failed, {load_asset_address}");  
        }     

       


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


    protected override void OnLoop()
    {
        base.OnLoop();

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