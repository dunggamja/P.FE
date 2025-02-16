using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumUIRootType
{
    None,

    //
    Screen,  // ��ü ȭ�鿡 ǥ�õǴ� UI
    HUD,     // HUD ���� �÷��� �� ǥ�õǴ� ����. 
}

public class UIManager : SingletonMono<UIManager>
{
    // canvas�� sort order ������ ���� �������� �� �ʿ�� ���°�?...

    [SerializeField] private Canvas m_canvas_root_screen = null;
    [SerializeField] private Canvas m_canvas_root_hud    = null;

    
    private void Awake()
    {
        var main_camera = Camera.main;
        if (main_camera == null)
        {
            Debug.LogError("UIManager: Main Camera is not found.");
        }
        else
        {
            // ���� ī�޶� �����ϵ��� ����.
            m_canvas_root_hud.worldCamera = main_camera;            
        }
    }

    
}
