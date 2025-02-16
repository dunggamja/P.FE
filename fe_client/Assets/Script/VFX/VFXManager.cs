using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class VFXManager : SingletonMono<VFXManager>
{
    private Dictionary<string, Queue<VFXObject>> m_vfx_pools = new();
    

    public async UniTask<VFXObject> CreateVFX(string _vfx_name)
    {
        VFXObject vfx_object = null;

        if (m_vfx_pools.TryGetValue(_vfx_name, out var pool) && pool.Count > 0)
        {
            vfx_object = pool.Dequeue();                        
        }
        else
        {
            var new_object = await AssetManager.Instance.InstantiateAsync(_vfx_name);
            if (new_object == null)
            {
                Debug.LogError($"VFXManager: CreateVFX failed to instantiate {_vfx_name}");
                return null;
            }

            vfx_object = new_object.TryAddComponent<VFXObject>();
            if (vfx_object == null)
            {
                Debug.LogError($"VFXManager: CreateVFX failed to add VFXObject component to {_vfx_name}");
                return null;
            }
        }

        // todo: vfx init
        vfx_object.transform.SetParent(transform);            
        return vfx_object;
    }

    public void ReleaseVFX(GameObject _vfx_object)
    {
        
    }

    // protected override void OnUpdate()
    // {
    //     base.OnUpdate();
    // }
}
