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

    


    
}
