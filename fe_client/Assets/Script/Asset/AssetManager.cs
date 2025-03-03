using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;



public class AssetManager : Singleton<AssetManager>
{
    Dictionary<string, AsyncOperationHandle> m_asset_handles = new (20);
   
    public async UniTask<GameObject> InstantiateAsync(string _asset_path, CancellationToken _cancel_token = default)
    {
        // , Action<GameObject> _callback = null
        var asset = await LoadAssetAsync<GameObject>(_asset_path, _cancel_token);
        if (asset == null)
        {
            return null;
        }

        var result  = GameObject.Instantiate(asset);
        if (result != null)
        {
            result.TryAddComponent<AssetSelfCleanup>();
        }

        return result;
    }

    public async UniTask<T> LoadAssetAsync<T>(string _asset_path, CancellationToken _cancel_token = default)
    {
        try
        {
            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(_asset_path);
            T result = await handle.ToUniTask(cancellationToken: _cancel_token);

            _cancel_token.ThrowIfCancellationRequested();

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                if (!m_asset_handles.ContainsKey(_asset_path))
                {
                    m_asset_handles.Add(_asset_path, handle);
                }
            }

            return result;
        }
        catch (OperationCanceledException)
        {
            Debug.LogError($"error, load asset, {_asset_path}"); 
            return default;
        }
    }

    public void ReleaseAsset(string _asset_path)
    {
        if (m_asset_handles.TryGetValue(_asset_path, out var handle))
        {
            m_asset_handles.Remove(_asset_path);
            Addressables.Release(handle);
        }
    }

    // public void InstantiateAsync(string _asset_path, Action<GameObject> _callback = null)
    // {        
    //     var task_async        = Addressables.InstantiateAsync(_asset_path);
    //     task_async.Completed += (e) => 
    //     {
    //         if (e.Status == AsyncOperationStatus.Succeeded)
    //         {
    //             if (e.Result)
    //             {
    //                 e.Result.TryAddComponent<AssetSelfCleanup>();
    //             }
    //         }
    //         if (_callback != null)
    //             _callback(e.Result);
    //     };
    // }


    // public async UniTask<T> LoadAssetAsync<T>(string _asset_path)
    // {   
    //     AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(_asset_path);  
        
    //     // ´ë±â.
    //     var result = await handle.Task;

    //     if (handle.Status == AsyncOperationStatus.Succeeded)
    //     {
    //         m_asset_handles.TryAdd(_asset_path, handle);       
    //     }

    //     return result;
    // }



}