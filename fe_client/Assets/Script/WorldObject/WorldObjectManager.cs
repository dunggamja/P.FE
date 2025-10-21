using System;
using System.Collections.Generic;
using Battle;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using R3;
using System.Threading;


public partial class WorldObjectManager : SingletonMono<WorldObjectManager>, IEventReceiver
{
    Dictionary<Int64, WorldObject> m_repository   = new(); 

    private List<WorldObject>      m_remove_queue = new(10);

    public WorldObject Seek(Int64 _id)      => m_repository.TryGetValue(_id, out var result) ? result : null;
    bool  Remove(Int64 _id)                 => m_repository.Remove(_id);
    bool  Insert(WorldObject _world_object) => (_world_object) ? m_repository.TryAdd(_world_object.ID, _world_object) : false;
    



    public async UniTask CreateObject(Int64 _id, CancellationToken _cancel_token = default)//, Action<bool, Int64> _on_result = null)
    {   
        // todo: cancellation token ó�� �߰�����.        

        // test code
        // var load_asset_address = AssetName.TEST_PREFAB;
        
        // unitask ���.

        try
        {
            var entity  = EntityManager.Instance.GetEntity(_id);
            if (entity == null)
            {
                Debug.LogError($"error, create object, {_id} - Entity null");
                return;
            }

            var asset_name = entity.AssetName;
            if (string.IsNullOrEmpty(asset_name))
            {
                asset_name = AssetName.TEST_PREFAB;
                Debug.LogWarning($"asset name is null, {_id} - use default asset name");
            }

            // TODO: 일단 임시로 부모 오브젝트 처리... 
            // 나중에는 루트 오브젝트를 넣어야 할 것 같다.
            var parent_object = this.transform;            

            // "오브젝트 생성"
            var new_object    = await AssetManager.Instance.InstantiateAsync(asset_name, parent_object, _cancel_token);
            
            // "로드 완료 후 처리"
            if (new_object != null)
            {
                var duplicate_object  = Seek(_id);

                // "중복 오브젝트 체크 or entity가 없는 경우 에러 처리"
                var is_error = (duplicate_object != null);
                if (is_error)
                {
                    Debug.LogWarning($"error, create object, {_id} - Duplicate: {duplicate_object != null}, Entity null: {entity == null}"); 
                    GameObject.Destroy(new_object);
                    return;
                }

                new_object.name = $"{_id.ToString("D10")}_{asset_name}";
                var new_actor   = new_object.TryAddComponent<WorldObject>();

                new_actor.Initialize(entity);

                Insert(new_actor);

                // �������� �� �̺�Ʈ ó��.

                // Debug.Log($"InstantiateAsync Success, {_id}");  
            }   
            else
            {
                Debug.Log($"InstantiateAsync Failed, {asset_name}");  
            }  
        }
        catch (OperationCanceledException)
        {
            Debug.LogError($"error, create object, {_id}"); 
        }
    }


    public void DeleteObject(Int64 _id)
    {
        var world_object =  Seek(_id);
        if (world_object == null)
            return;


        Remove(_id);

        // "비활성화 후 삭제 큐에 넣어서 나중에 처리"
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

        // "오브젝트 제거 처리"
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
    //     // TODO: Addressable ����غ���.
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