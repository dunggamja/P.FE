using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class ActorManager : SingletonMono<ActorManager>
{
    Dictionary<Int64, Actor> m_repository = new();

    public Actor Seek(Int64 _id) => m_repository.TryGetValue(_id, out var result) ? result : null;

    public async void Create(Int64 _id)
    {
        
        var load_asset_address = "test";
        var async_instantiate  = Addressables.InstantiateAsync(load_asset_address);

        await async_instantiate.Task;

        var new_object = async_instantiate.Result;
        if (new_object)
        {
            new_object.name = $"{_id.ToString("D10")}_{load_asset_address}";
        }
        else
        {
            Debug.Log($"InstantiateAsync Failed, {async_instantiate.Status}");
        }

        
        
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