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
}

public partial class VFXManager : SingletonMono<VFXManager>, IEventReceiver
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
            // 시리얼 넘버 증가.
            ++m_vfx_serial;

            // 시리얼 넘버가 0이면 1로 변경.
            if (m_vfx_serial <= 0)
                m_vfx_serial = 1;
            
            // 중복 체크.
        } while (m_vfx_repository.ContainsKey(m_vfx_serial));

        return m_vfx_serial;
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


    public int CreateVFXAsync(
        string     _vfx_name, 
        Transform  _parent       = null, 
        Vector3    _position     = default, 
        Quaternion _rotation     = default, 
        float      _scale        = 1f,
        bool       _auto_release = false)
    {
        // TODO: 자동 이펙트가 릴리즈 되는 기능 추가.

        // 시리얼 넘버 생성.
        var serial_number = GenerateSerial();

        // 파라미터 설정.
        var param = new VFXCreateParam()
        {
            SerialNumber = serial_number,
            VFXName      = _vfx_name,
            Parent       = (_parent != null) ? _parent : m_vfx_use_root,
            Position     = _position,
            Rotation     = _rotation,
            Scale        = _scale
        };

        // 이펙트 생성. (비동기)
        CreateVFXAsync(param).Forget();

        // 시리얼 넘버 반환.
        return serial_number;
    }

    public bool ReserveReleaseVFX(int _serial_number)
    {
        if (_serial_number == 0)
            return false;

        return m_vfx_release_list.Add(_serial_number);
    }

    VFXObject SeekVFX(int _serial_number)
    {
        if (_serial_number == 0)
            return null;

        return m_vfx_repository.TryGetValue(_serial_number, out var vfx_object) ? vfx_object : null;
    }
    

    async UniTask<VFXObject> CreateVFXAsync(VFXCreateParam _param, CancellationToken _cancel_token = default)
    {
        // 풀에서 가져오기.
        VFXObject vfx_object = AcquireFromPool(_param.VFXName);

        // 풀에 없으면 생성.
        if (vfx_object == null)
        {
            // 프리팹 로드 대기.
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

        // 삭제 예정 목록에 있으면 생성하지 않습니다.
        if (m_vfx_release_list.Contains(_param.SerialNumber))
        {
            m_vfx_release_list.Remove(_param.SerialNumber);
            ReturnToPool(_param.VFXName, vfx_object);
            return null;
        }

        // 레포지토리에 추가.
        m_vfx_repository.Add(_param.SerialNumber, vfx_object);

        // 오브젝트 초기화
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
        var release_list = HashSetPool<int>.Acquire();

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
        HashSetPool<int>.Return(release_list);
    }



}
