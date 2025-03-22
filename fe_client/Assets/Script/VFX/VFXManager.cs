using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

struct VFXCreateParam
{
    public int        SerialNumber { get; set; }
    public string     VFXName      { get; set; }
    public Transform  Parent       { get; set; }
    public Vector3    Position     { get; set; }
    public Quaternion Rotation     { get; set; }
    public float      Scale        { get; set; }

    // public static VFXCreateParam Default
    // {
    //     get
    //     {
    //         return new VFXCreateParam()
    //         {
    //             SerialNumber = 0,
    //             VFXName      = string.Empty,
    //             Parent       = null,
    //             Position     = Vector3.zero,
    //             Rotation     = Quaternion.identity,
    //             Scale        = 1f
    //         };
    //     }
    // }

}

public class VFXManager : SingletonMono<VFXManager>
{
    [SerializeField]
    private Transform m_vfx_pool_root = null;
    [SerializeField]
    private Transform m_vfx_use_root  = null;


    private Dictionary<string, Queue<VFXObject>> m_vfx_pools        = new();
    private Dictionary<int, VFXObject>           m_vfx_repository   = new();
    private HashSet<int>                         m_vfx_release_list = new();

    private int m_vfx_serial = 0;

    private int GenerateSerial()
    {
        do
        {
            // �ø��� �ѹ� ����.
            ++m_vfx_serial;

            // �ø��� �ѹ��� 0�̸� 1�� ����.
            if (m_vfx_serial <= 0)
                m_vfx_serial = 1;
            
            // �ߺ� üũ.
        } while (m_vfx_repository.ContainsKey(m_vfx_serial));

        return m_vfx_serial;
    }

    public int CreateVFXAsync(string _vfx_name, Transform _parent = null, Vector3 _position = default, Quaternion _rotation = default, float _scale = 1f)
    {
        // TODO: �ڵ� ����Ʈ�� ������ �Ǵ� ��� �߰�.

        // �ø��� �ѹ� ����.
        var serial_number = GenerateSerial();

        // �Ķ���� ����.
        var param = new VFXCreateParam()
        {
            SerialNumber = serial_number,
            VFXName      = _vfx_name,
            Parent       = (_parent != null) ? _parent : m_vfx_use_root,
            Position     = _position,
            Rotation     = _rotation,
            Scale        = _scale
        };

        // ����Ʈ ����. (�񵿱�)
        CreateVFXAsync(param).Forget();

        // �ø��� �ѹ� ��ȯ.
        return serial_number;
    }

    public bool ReserveReleaseVFX(int _serial_number)
    {
        if (_serial_number == 0)
            return false;

        return m_vfx_release_list.Add(_serial_number);
    }
    

    async UniTask<VFXObject> CreateVFXAsync(VFXCreateParam _param, CancellationToken _cancel_token = default)
    {
        // Ǯ���� ��������.
        VFXObject vfx_object = AcquireFromPool(_param.VFXName);

        // Ǯ�� ������ ����.
        if (vfx_object == null)
        {
            // ������ �ε� ���.
            var new_object = await AssetManager.Instance.InstantiateAsync(_param.VFXName, _cancel_token);
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

        // �������丮�� �߰�.
        m_vfx_repository.Add(_param.SerialNumber, vfx_object);

        // ������Ʈ �ʱ�ȭ
        vfx_object.OnCreate(
            _param.SerialNumber, 
            _param.VFXName, 
            _param.Parent,
            _param.Position,
            _param.Rotation,
            _param.Scale);

        
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

    protected override void OnLoop()
    {
        base.OnLoop();

        OnLoop_ReleaseVFX();
    }


    void OnLoop_ReleaseVFX()
    {
        if (m_vfx_release_list.Count <= 0)
            return;

        // Ǯ���� ��������
        var release_list = HashSetPool<int>.Acquire();

        // ������ ��� ����.
        release_list.UnionWith(m_vfx_release_list);
        m_vfx_release_list.Clear();

        // ������ ��� ó��.
        foreach (var serial_number in release_list)
        {
            if (m_vfx_repository.TryGetValue(serial_number, out var vfx_object))
            {
                ReleaseVFX(vfx_object);
            }
        }

        // Ǯ�� ��ȯ.
        HashSetPool<int>.Return(release_list);
    }
}
