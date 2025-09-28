using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public enum EnumVFXAttachmentType
{
    World,      // ���� ��ǥ�� ����
    Child,      // Ư�� ������Ʈ�� �ڽ����� ����
    Track       // Ư�� ������Ʈ�� ����
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

        // TODO: �ڵ� ����Ʈ�� ������ �Ǵ� ��� �߰�.

        // �ø��� �ѹ� ����.
        var serial_number = GenerateSerial();

        // ����Ʈ ����. (�񵿱�)
        CreateVFXAsync(serial_number, _param).Forget();

        // �ø��� �ѹ� ��ȯ.
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

        // Ǯ���� ��������.
        VFXObject vfx_object = AcquireFromPool(_param.VFXName);

        // Ǯ�� ������ ����.
        if (vfx_object == null)
        {
            // ������ �ε� ���.
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

        // ���� ���� ��Ͽ� ������ ���� ���.
        if (m_vfx_release_list.Contains(_serial_number))
        {
            // m_vfx_release_list.Remove(_serial_number);
            ReturnToPool(_param.VFXName, vfx_object);

            vfx_object = null;
        }
        else
        {
            // �������丮�� �߰�.
            m_vfx_repository.Add(_serial_number, vfx_object);

            // ������Ʈ �ʱ�ȭ
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

        // ������Ʈ �ʱ�ȭ.
        _vfx_object.OnRelease();

        // Ǯ�� ��ȯ.
        ReturnToPool(vfx_name, _vfx_object);
        
        // �������丮���� ����.
        m_vfx_repository.Remove(serial_number);
        // ���� ��Ͽ����� ����.
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

        // Ǯ ��Ʈ ������ �ű��
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

        // Ǯ���� ��������
        var release_list = HashSetPool<Int64>.Acquire();

        // ������ ��� ����.
        release_list.UnionWith(m_vfx_release_list);

        // ������ ��� ó��.
        foreach (var serial_number in release_list)
        {
            if (m_vfx_repository.TryGetValue(serial_number, out var vfx_object))
            {
                ReleaseVFX(vfx_object);
            }
        }

        // Ǯ�� ��ȯ.
        HashSetPool<Int64>.Return( release_list);
    }



}
