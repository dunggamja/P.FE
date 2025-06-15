using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum EnumVFXAttachmentType
{
    World,      // 월드 좌표에 고정
    Child,      // 특정 오브젝트의 자식으로 부착
    Track       // 특정 오브젝트를 추적
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

        // TODO: 자동 이펙트가 릴리즈 되는 기능 추가.

        // 시리얼 넘버 생성.
        var serial_number = GenerateSerial();

        // 이펙트 생성. (비동기)
        CreateVFXAsync(serial_number, _param).Forget();

        // 시리얼 넘버 반환.
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

        // 풀에서 가져오기.
        VFXObject vfx_object = AcquireFromPool(_param.VFXName);

        // 풀에 없으면 생성.
        if (vfx_object == null)
        {
            // 프리팹 로드 대기.
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

        // 삭제 예정 목록에 있으면 생성 취소.
        if (m_vfx_release_list.Contains(_serial_number))
        {
            // m_vfx_release_list.Remove(_serial_number);
            ReturnToPool(_param.VFXName, vfx_object);

            vfx_object = null;
        }
        else
        {
            // 레포지토리에 추가.
            m_vfx_repository.Add(_serial_number, vfx_object);

            // 오브젝트 초기화
            vfx_object.OnCreate(
                _serial_number, 
                _param);
        }



        // 반환.
        ObjectPool<T>.Return(_param);

        return vfx_object;
    }

    void ReleaseVFX(VFXObject _vfx_object)
    {
        if (_vfx_object == null)
            return;

        var serial_number = _vfx_object.SerialNumber;
        var vfx_name      = _vfx_object.VFXName;

        // 오브젝트 초기화.
        _vfx_object.OnRelease();

        // 풀에 반환.
        ReturnToPool(vfx_name, _vfx_object);
        
        // 레포지토리에서 제거.
        m_vfx_repository.Remove(serial_number);
        // 삭제 목록에서도 제거.
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

        // 풀 루트 밑으로 옮기기
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

        // 풀에서 가져오기
        var release_list = HashSetPool<Int64>.Acquire();

        // 릴리즈 목록 복사.
        release_list.UnionWith(m_vfx_release_list);

        // 릴리즈 목록 처리.
        foreach (var serial_number in release_list)
        {
            if (m_vfx_repository.TryGetValue(serial_number, out var vfx_object))
            {
                ReleaseVFX(vfx_object);
            }
        }

        // 풀에 반환.
        HashSetPool<Int64>.Return(release_list);
    }



}
