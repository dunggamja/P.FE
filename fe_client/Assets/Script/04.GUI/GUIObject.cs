using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;


public enum EnumGUIType
{
    None,

    //
    Screen,  // ��ü ȭ�鿡 ǥ�õǴ� UI
    Popup,   // �˾� ������ UI
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
  

    protected CompositeDisposable m_disposables = new();

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
                // UI OnLoop 30프레임 
                await UniTask.WaitForSeconds(1f/30f);

                // 닫히고 있으면 중단 처리.
                if (IsClosing)
                    break;

                // Loop 처리.
                OnLoop();
            } 
        }
        catch (OperationCanceledException)
        {
            // Task 중단 시 에러로그. (- CancellationToken 추가)
            Debug.LogWarning($"[{gameObject.name}] Task was cancelled");
        }
    }


    protected virtual void OnClear()
    {
        m_disposables.Clear();
    }

}

public abstract class GUIElement : GUIBase
{
    // button, scrollview, etc.
    // 화면 요소, GUI 요소에 대한 기본 클래스.

    public Int64 OwnerGUIID
    {
        get
        {
            var owner_gui = transform.GetComponentInParent<GUIPage>();
            if (owner_gui != null)
                return owner_gui.ID;

            return 0;
        }
    }

    virtual protected void OnDestroy()
    {
        // TODO: 클래스 소멸 시 처리 필요한데.. OnDestroy 말고 다른 곳에 있는게 좋을거 같은...
        // Clear 처리 필요.
        OnClear();
    }


}
