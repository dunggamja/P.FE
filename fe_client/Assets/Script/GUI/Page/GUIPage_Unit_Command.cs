using System;
using UnityEngine;
using UnityEngine.UI;
using R3;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Battle;

[EventReceiver(
    typeof(GUI_Menu_MoveEvent), 
    typeof(GUI_Menu_SelectEvent),
    typeof(Battle_Scene_ChangeEvent)
    )]
public class GUIPage_Unit_Command : GUIPage, IEventReceiver
{
    public class PARAM : GUIOpenParam
    {
        public Int64 EntityID { get; private set; }
        public override EnumGUIType GUIType => EnumGUIType.Screen;

        private PARAM(Int64 _entity_id) 
        : base(
            // id      
            GUIPage.GenerateID(),    

            // asset path
            "gui/page/unit_command",   

            // is input enabled
            true                     
            )      
        { 
            EntityID = _entity_id;  
        }


        static public PARAM Create(Int64 _entity_id)
        {
            return new PARAM(_entity_id);
        }
    }

    struct MENU_ITEM_DATA
    {
        public enum EnumMenuType
        {
            None,
            Attack,
            Skill,
            Item,
            Wait,
        }

        public int          Index    { get; private set; }
        public EnumMenuType MenuType { get; private set; }
        // public string       Text  { get; private set; } = "";
        public MENU_ITEM_DATA(int _index, EnumMenuType _type)
        {
            Index    = _index;
            MenuType = _type;
        }

        public LocalizeKey GetLocalizeKey()
        {
            var table = string.Empty;
            var key   = string.Empty;

            switch (MenuType)
            {
                case EnumMenuType.Attack: 
                    table = "localization_base";
                    key   = "ui_menu_attack";                    
                    break;
                case EnumMenuType.Wait:  
                    table = "localization_base";
                    key   = "ui_menu_wait";
                    break;
            }

            return LocalizeKey.Create(table, key);
        }

        public static MENU_ITEM_DATA Empty => new MENU_ITEM_DATA(0, EnumMenuType.None);
    }

    


    [SerializeField]
    private RectTransform                 m_grid_menu_root_bg_rect;

    [SerializeField]
    private RectTransform                 m_grid_menu_root_rect;

    [SerializeField]
    private GridLayoutGroup               m_grid_menu_root;

    [SerializeField]
    private GUIElement_Grid_Item_MenuText m_grid_menu_item;


    private Int64                         m_entity_id = 0;           
    private MENU_ITEM_DATA[]              m_menu_item_datas;
    private (bool init, Vector2 value)    m_grid_menu_padding      = (false, Vector2.zero);  
    private BehaviorSubject<int>          m_selected_index_subject = new(0);

    private MENU_ITEM_DATA SelectedItemData
    {
        get
        {
            var cur_index = m_selected_index_subject.Value;
            if (cur_index < 0 || cur_index >= m_menu_item_datas.Length)
                return MENU_ITEM_DATA.Empty;

            return m_menu_item_datas[cur_index];
        }
    }


    public void OnReceiveEvent(IEventParam _event)
    {
        switch (_event)
        {
            case GUI_Menu_MoveEvent menu_move_event:
                OnReceiveEvent_GUI_Menu_MoveEvent(menu_move_event);
                break;

            case GUI_Menu_SelectEvent menu_select_event:
                OnReceiveEvent_GUI_Menu_SelectEvent(menu_select_event);
                break;

            case Battle_Scene_ChangeEvent:
                GUIManager.Instance.CloseUI(ID);
                break;
        }
        // throw new NotImplementedException();
    }
    

    protected override void OnOpen(GUIOpenParam _param)
    {
        var param   = _param as PARAM;
        m_entity_id = param?.EntityID ?? 0;

        // �޴� ������ ����
        UpdateMenuItems();  

        // ���̾ƿ� ����
        UpdateLayout();
    }

    protected override void OnLoop()
    {
        base.OnLoop();

        // ���� ǥ��.
        UpdateDrawRange();
    }

    protected override void OnClose()
    {
        // ���� ǥ�� ����.
        BattleSystemManager.Instance.DrawRange.Clear();
    }

    protected override void OnPostProcess_Close()
    {
        
    }



    private void UpdateMenuItems()
    {
         // TESTCODE: �޴� ������ ������ ����.
        m_menu_item_datas = new MENU_ITEM_DATA[]
        {
            new MENU_ITEM_DATA(0, MENU_ITEM_DATA.EnumMenuType.Attack),
            new MENU_ITEM_DATA(1, MENU_ITEM_DATA.EnumMenuType.Wait),
            // new MENU_ITEM_DATA(2, "Move"),
            // new MENU_ITEM_DATA(3, "Skill"),
            // new MENU_ITEM_DATA(4, "Item")
        };

        // �޴� ������ ����.
        for (int i = 0; i < m_menu_item_datas.Length; i++)
        {
            var localizeKey  = m_menu_item_datas[i].GetLocalizeKey();
            var text_subject = LocalizationManager.Instance.GetTextObservable(localizeKey.Table, localizeKey.Key);

            // TODO: ������Ʈ Ǯ���ϴ°� �� ������?
            var clonedItem   = Instantiate(m_grid_menu_item, m_grid_menu_root.transform);
            
            clonedItem.Initialize(i, m_selected_index_subject, text_subject);
        }

        // �ʱ� ���� �ε��� ���� (0�� �ε��� ����)
        m_selected_index_subject.OnNext(0);
    }

    private void UpdateLayout()
    {
        // �޴� ������ ������ ���� ������ ����.
        {
            if (m_grid_menu_padding.init == false)
            {
                var padding = m_grid_menu_root_bg_rect.sizeDelta - m_grid_menu_root_rect.sizeDelta;
                padding.x   = Mathf.Abs(padding.x);
                padding.y   = Mathf.Abs(padding.y);

                m_grid_menu_padding = (true, padding);
            }

            var padding_height = m_grid_menu_root.padding.top + m_grid_menu_root.padding.bottom;
            var child_count    = m_grid_menu_root.transform.childCount;
            var cell_height    = m_grid_menu_root.cellSize.y;
            var spacing_height = m_grid_menu_root.spacing.y;
            var total_height   = (child_count * cell_height) + Mathf.Max(0, spacing_height * (child_count - 1)) + padding_height;

            m_grid_menu_root_rect.sizeDelta    = new Vector2(m_grid_menu_root_rect.sizeDelta.x, total_height);
            m_grid_menu_root_bg_rect.sizeDelta = new Vector2(m_grid_menu_root_bg_rect.sizeDelta.x, total_height + m_grid_menu_padding.value.y);
        }
    }

    // // �ε��� ���� �޼���
    // public void SetSelectedIndex(int index)
    // {
    //     m_selected_index_subject.OnNext(index);
    // }

    
    // �޴� �̵� �̺�Ʈ ����.
    void OnReceiveEvent_GUI_Menu_MoveEvent(GUI_Menu_MoveEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        // �̵� ������ ������ ����.
        if (_event.MoveDirection.y  == 0)
            return;

        
        // �̵� ���⿡ ���� �޴� ������ ����.
        var add_index = _event.MoveDirection.y > 0 ? -1 : +1;
        var cur_index = m_selected_index_subject.Value;
        var new_index = cur_index + add_index;

        // �ε��� ���� üũ.
        new_index     = Math.Clamp(new_index, 0, m_menu_item_datas.Length - 1);
        
        // �ε��� ����.
        m_selected_index_subject.OnNext(new_index);

        // ���� ǥ��.
        UpdateDrawRange();
    }

    void OnReceiveEvent_GUI_Menu_SelectEvent(GUI_Menu_SelectEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        // ���� �̺�Ʈ ó��.        
        switch (SelectedItemData.MenuType)
        {
            case MENU_ITEM_DATA.EnumMenuType.Attack:
            {
                // ���� GUI ����.
                GUIManager.Instance.OpenUI(
                    GUIPage_Unit_Command_Attack.PARAM.Create(m_entity_id)
                    );
            }
                break;
            case MENU_ITEM_DATA.EnumMenuType.Wait:                
                break;
            case MENU_ITEM_DATA.EnumMenuType.Skill:
                break;
            case MENU_ITEM_DATA.EnumMenuType.Item:
                break;                
        }

        
    }


    protected override void OnInputFocusChanged(bool _focused)
    {
        base.OnInputFocusChanged(_focused);

        if (_focused)
        {
            // ��Ŀ��.
            Show();
        }
        else
        {
            // ��Ŀ�� ����.
            Hide();
        }
    }

    private void UpdateDrawRange()
    {
        if (IsInputFocused == false)
            return;

        int draw_flag = 0;
        switch (SelectedItemData.MenuType)
        {
            case MENU_ITEM_DATA.EnumMenuType.Attack:
                draw_flag = (int)Battle.MoveRange.EnumDrawFlag.AttackRange;
                break;
        }

        // ���� ǥ��.
        BattleSystemManager.Instance.DrawRange.DrawRange(
            _draw_flag: draw_flag,
            _entityID: m_entity_id,
            _use_base_position: false);

    }

}
