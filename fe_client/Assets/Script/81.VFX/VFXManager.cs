using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum EnumVFXAttachmentType
{
    World,      // 월드에 위치한다.
    Child,      // 오브젝트 하위에 붙는다.
    Track       // 오브젝트를 따라다닌다.
}




public partial class VFXManager : SingletonMono<VFXManager>, IEventReceiver
{
    [SerializeField]
    private Transform m_vfx_pool_root = null;
    [SerializeField]
    private Transform m_vfx_use_root  = null;


    private Dictionary<string, Queue<VFXObject>> m_vfx_pools        = new();
    private Dictionary<Int64, VFXObject>         m_vfx_repository   = new();
    private HashSet<Int64>                       m_vfx_release_list = new();

    private Int64 m_vfx_serial = 0;

    private Int64 GenerateSerial()
    {
        return ++m_vfx_serial;
    }

    protected override void OnInitialize()
    {
        base.OnInitialize();

        EventDispatchManager.Instance.AttachReceiver(this);
    }


    protected override void OnLoop()
    {
        base.OnLoop();

        OnLoop_ReleaseVFX();
    }


    protected override void OnRelease(bool _is_shutdown)
    {
        base.OnRelease(_is_shutdown);

        if (!_is_shutdown)
        {
            EventDispatchManager.Instance.DetachReceiver(this);
        }
    }


    public Int64 CreateVFXAsync<T>(T _param) where T : VFXObject.Param, new()
    {
        if (_param == null)
            return 0;

        // TODO: 오브젝트 풀링 추가.

        // 시리얼 번호 생성.
        var serial_number = GenerateSerial();

        // 오브젝트 생성.
        CreateVFXAsync(serial_number, _param).Forget();

        // 오브젝트 번호 반환.
        return serial_number;
    }

    public bool ReserveReleaseVFX(Int64 _serial_number)
    {
        if (_serial_number == 0)
            return false;

        return m_vfx_release_list.Add(_serial_number);
    }

    VFXObject SeekVFX(Int64 _serial_number)
    {
        if (_serial_number == 0)
            return null;

        return m_vfx_repository.TryGetValue(_serial_number, out var vfx_object) ? vfx_object : null;
    }
    

    async UniTask<VFXObject> CreateVFXAsync<T>(Int64 _serial_number, T _param, CancellationToken _cancel_token = default)
        where T : VFXObject.Param, new()
    {
        if (_param == null)
            return null;

        // 오브젝트 풀링 오브젝트 취득.
        VFXObject vfx_object = AcquireFromPool(_param.VFXName);

        // 오브젝트 풀링 오브젝트 없으면 생성.
        if (vfx_object == null)
        {
            // 오브젝트 생성.
            var new_object = await AssetManager.Instance.InstantiateAsync(_param.VFXName, _param.VFXRoot, _cancel_token);
            if (new_object == null)
            {
                Debug.LogError($"VFXManager: CreateVFX failed to instantiate {_param.VFXName}");
                return null;
            }

            vfx_object = new_object.TryAddComponent<VFXObject>();
            if (vfx_object == null)
            {
                Debug.LogError($"VFXManager: CreateVFX failed to add VFXObject component to {_param.VFXName}");
                return null;
            }
        }

        // 오브젝트 릴리즈 리스트에 있으면 풀링 오브젝트 반환.
        if (m_vfx_release_list.Contains(_serial_number))
        {
            // m_vfx_release_list.Remove(_serial_number);
            ReturnToPool(_param.VFXName, vfx_object);

            vfx_object = null;
        }
        else
        {
            // 오브젝트 리포지토리에 추가.
            m_vfx_repository.Add(_serial_number, vfx_object);

            // 오브젝트 생성.
            vfx_object.OnCreate(
                _serial_number, 
                _param);
        }



        // ��ȯ.
        ObjectPool<T>.Return( _param);

        return vfx_object;
    }

    void ReleaseVFX(VFXObject _vfx_object)
    {
        if (_vfx_object == null)
            return;

        var serial_number = _vfx_object.SerialNumber;
        var vfx_name      = _vfx_object.VFXName;

        // 오브젝트 릴리즈.
        _vfx_object.OnRelease();

        // 오브젝트 풀링 오브젝트 반환.
        ReturnToPool(vfx_name, _vfx_object);
        
        // 오브젝트 리포지토리에서 제거.
        m_vfx_repository.Remove(serial_number);
        // 오브젝트 릴리즈 리스트에서 제거.
        m_vfx_release_list.Remove(serial_number);
    }





    VFXObject AcquireFromPool(string _vfx_name)
    {
        if (m_vfx_pools.TryGetValue(_vfx_name, out var pool) && pool.Count > 0)
        {
            return pool.Dequeue();
        }

        return null;
    }


    

    void ReturnToPool(string _vfx_name, VFXObject _vfx_object)
    {
        if (_vfx_object == null)
            return;

        if (!m_vfx_pools.TryGetValue(_vfx_name, out var pool))
        {
            pool = new Queue<VFXObject>();
            m_vfx_pools.Add(_vfx_name, pool);
        }

        pool.Enqueue(_vfx_object);

        // 오브젝트 풀링 오브젝트 셋팅.
        _vfx_object.gameObject.SetActive(false);
        _vfx_object.transform.SetParent(m_vfx_pool_root);
        _vfx_object.transform.localPosition = Vector3.zero;
        _vfx_object.transform.localRotation = Quaternion.identity;
        _vfx_object.transform.localScale    = Vector3.one;
    }





    void OnLoop_ReleaseVFX()
    {
        if (m_vfx_release_list.Count <= 0)
            return;

        // 오브젝트 릴리즈 리스트 취득.
        using var release_list = HashSetPool<Int64>.AcquireWrapper();

        // 오브젝트 릴리즈 리스트 추가.
        release_list.Value.UnionWith(m_vfx_release_list);

        // 오브젝트 릴리즈 리스트 처리.
        foreach (var serial_number in release_list.Value)
        {
            if (m_vfx_repository.TryGetValue(serial_number, out var vfx_object))
            {
                ReleaseVFX(vfx_object);
            }
        }
    }



}
