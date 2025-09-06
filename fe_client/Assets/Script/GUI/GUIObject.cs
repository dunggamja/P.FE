using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;


public enum EnumGUIType
{
    None,

    //
    Screen,  // ��ü ȭ�鿡 ǥ�õǴ� UI
    // Popup,   // �˾� ������ UI
    HUD,     // ���� �����̽��� ǥ�õǴ� UI
}

public enum EnumGUIState
{
    None,
    Opening, // ������ ��   
    Opened,  // ����
    Closing, // �ݴ� ��
    Closed,  // ����
}


public abstract class GUIBase : MonoBehaviour//, IUIProperty
{
   
    // private Vector3    m_init_local_position = Vector3.zero;
    // private Quaternion m_init_local_rotation = Quaternion.identity;
    // private Vector3    m_init_local_scale = Vector3.one;

    // virtual protected void Awake()
    // {
    //     m_init_local_position = transform.localPosition;
    //     m_init_local_rotation = transform.localRotation;
    //     m_init_local_scale    = transform.localScale;
    // }

    // protected void Init_Transform(Transform _parent)
    // {
    //     transform.SetParent(_parent);
    //     transform.localPosition = m_init_local_position;
    //     transform.localRotation = m_init_local_rotation;
    //     transform.localScale    = m_init_local_scale;
    // }

    protected bool IsClosing { get; set; } = false;

    virtual protected void Awake()
    {
        StartLoop().Forget();
    }


    virtual protected void OnLoop()
    {

    }

    private async UniTask StartLoop()
    {  
        try
        {
            while (true)
            {
                // UI ���� 30������ - CancellationToken �߰�
                await UniTask.WaitForSeconds(1f/30f);

                // �ݱ� ó����.
                if (IsClosing)
                    break;

                // ���⼭ cancel üũ?
                OnLoop();
            } 
        }
        catch (OperationCanceledException)
        {
            // GameObject�� �ı��Ǹ� ���⼭ Task ����
            Debug.LogWarning($"[{gameObject.name}] Task was cancelled");
        }
    }

}

public abstract class GUIElement : GUIBase
{
    // button, scrollview, etc.
    // ���� �� ������, GUI ���� ��ü���� Element �� �з��غ� ����.

    virtual protected void OnDestroy()
    {
        // TODO: �̰��� �ӽ� �ڵ���....
        // Clear�� ��� ó���ؾ� ���� ���� ������ �ʾ���.
        Clear();
    }

    protected abstract void Clear();
}
