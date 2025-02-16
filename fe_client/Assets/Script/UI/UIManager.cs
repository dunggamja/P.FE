using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumUIRootType
{
    None,

    //
    Screen,  // 전체 화면에 표시되는 UI
    HUD,     // HUD 게임 플레이 중 표시되는 정보. 
}

public class UIManager : SingletonMono<UIManager>
{
    // canvas를 sort order 순서에 따라 복수개를 둘 필요는 없는가?...

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
            // 메인 카메라를 참조하도록 설정.
            m_canvas_root_hud.worldCamera = main_camera;            
        }
    }

    
}
