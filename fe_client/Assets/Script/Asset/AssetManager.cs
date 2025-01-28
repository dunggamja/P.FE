using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class AssetManager : Singleton<AssetManager>
{
    Dictionary<string, AsyncOperationHandle> m_asset_handles = new (20);
   
    public void InstantiateAsync(string _asset_path, Action<GameObject> _callback = null)
    {
        
        var task_async        = Addressables.InstantiateAsync(_asset_path);
        task_async.Completed += (e) => 
        {
            if (e.Status == AsyncOperationStatus.Succeeded)
            {
                if (e.Result)
                {
                    e.Result.TryAddComponent<AssetSelfCleanup>();
                }
            }

            if (_callback != null)
                _callback(e.Result);
        };
    }

    public void LoadAssetAsync<T>(string _asset_path, Action<T> _callback = null)
    {   
        var task_async        = Addressables.LoadAssetAsync<T>(_asset_path);   
        task_async.Completed += (e) => 
        {   
            if (e.Status == AsyncOperationStatus.Succeeded)
            {
                m_asset_handles.TryAdd(_asset_path, task_async);            
            }

            if (_callback != null)
                _callback(e.Result);
        };     
    }

    public void ReleaseAsset(string _asset_path)
    {
        if (m_asset_handles.TryGetValue(_asset_path, out var handle))
        {
            m_asset_handles.Remove(_asset_path);
            Addressables.Release(handle);
        }
    }

}