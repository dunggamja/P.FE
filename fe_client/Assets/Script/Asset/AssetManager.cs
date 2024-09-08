using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class AssetManager : Singleton<AssetManager>
{
   
    public void InstantiateAsync(string _asset_path, Action<GameObject> _callback = null)
    {
        var task_async        = Addressables.InstantiateAsync(_asset_path);
        task_async.Completed += (e) => 
        {
            if (_callback != null)
                _callback(e.Result);
        };
    }

    public void LoadAssetAsync(string _asset_path, Action<object> _callback = null)
    {        
        var task_async        = Addressables.LoadAssetAsync<object>(_asset_path);   
        task_async.Completed += (e) => 
        {
            if (_callback != null)
                _callback(e.Result);
        };     
    }

    
}