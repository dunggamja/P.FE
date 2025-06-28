using System;
using UnityEngine;
using UnityEngine.UI;
using R3;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[EventReceiver(
    typeof(GUI_Menu_MoveEvent), 
    typeof(GUI_Menu_SelectEvent)
    )]
public class GUIPage_Unit_Command : GUIPage, IEventReceiver
{
    public class PARAM : GUIOpenParam
    {
        public Int64 EntityID { get; private set; }

        private PARAM(Int64 _entity_id) 
        : base(
            GUIPage.GenerateID(),    // id      
            "gui/page/unit_command", // asset path
            EnumGUIType.Screen)      // gui type
        { 
            EntityID = _entity_id;  
        }


        static public PARAM Create(Int64 _entity_id)
        {
            return new PARAM(_entity_id);
        }
    }

    class MENU_ITEM_DATA
    {
        public enum EnumMenuType
        {
            None,
            Attack,
            Skill,
            Item,
            Wait,
        }

        public int          Index { get; private set; } = 0;
        public EnumMenuType MenuType { get; private set; } = EnumMenuType.None;
        // public string       Text  { get; private set; } = "";
        public MENU_ITEM_DATA(int _index, EnumMenuType _type)
        {
            Index    = _index;
            MenuType = _type;
        }

        public (string table, string key) GetLocalizeKey()
        {
            switch (MenuType)
            {
                // TODO: �� ������ �Ϸ��� ��� �ؾ� �ұ�?
                case EnumMenuType.Attack: return ("localization_base", "ui_menu_attack");
                case EnumMenuType.Wait:   return ("localization_base", "ui_menu_wait");
                case EnumMenuType.Skill:  return ("localization_base", "ui_menu_skill");
                case EnumMenuType.Item:   return ("localization_base", "ui_menu_item");                
            }

            return ("", "");
        }
    }


    [SerializeField]
    private RectTransform                 m_grid_menu_root_bg_rect;

    [SerializeField]
    private RectTransform                 m_grid_menu_root_rect;

    [SerializeField]
    private GridLayoutGroup               m_grid_menu_root;

    [SerializeField]
    private GUIElement_Grid_Item_MenuText m_grid_menu_item;


    private Int64                       m_entity_id = 0;           
    private MENU_ITEM_DATA[]            m_menu_item_datas;
    private (bool init, Vector2 value)  m_grid_menu_padding = (false, Vector2.zero);

    private BehaviorSubject<int>        m_selected_index_subject    = new(0);


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


        // var entity = EntityManager.Instance.Find(_param.EntityID);
        
        // var entity = Battle.EntityManager.Instance.Find(_param);
        // if (entity == null)
        // {
        //     Debug.LogError($"Entity not found: {_param.EntityID}");
        //     return;
        // }
    }

    protected override void OnClose()
    {
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
            var (table, key) = m_menu_item_datas[i].GetLocalizeKey();
            var text_subject = LocalizationManager.Instance.GetTextObservable(table, key);

            // TODO: ������Ʈ Ǯ���ϴ°� �� ������?
            var clonedItem   = Instantiate(m_grid_menu_item, m_grid_menu_root.transform);
            
            clonedItem.Initialize(i, m_selected_index_subject, text_subject);

            // clonedItem.SetText(m_menu_item_datas[i].GetMenuText());
            clonedItem.gameObject.SetActive(true);
        }

        // �ʱ� ���� �ε��� ���� (0�� �ε��� ����)
        SetSelectedIndex(0);
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

    // �ε��� ���� �޼���
    public void SetSelectedIndex(int index)
    {
        m_selected_index_subject.OnNext(index);
    }

    
    // �޴� �̵� �̺�Ʈ ����.
    void OnReceiveEvent_GUI_Menu_MoveEvent(GUI_Menu_MoveEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        // �̵� ������ ������ ����.
        if (_event.MoveDirection.y  == 0)
            return;

        
        var add_index = _event.MoveDirection.y > 0 ? -1 : +1;
        var cur_index = m_selected_index_subject.Value;
        var new_index = cur_index + add_index;

        new_index = Math.Clamp(new_index, 0, m_menu_item_datas.Length - 1);
        // �̵� ���⿡ ���� �޴� ������ ����.
        
        SetSelectedIndex(new_index);
    }

    void OnReceiveEvent_GUI_Menu_SelectEvent(GUI_Menu_SelectEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        // ���� �̺�Ʈ ó��.
        var cur_index = m_selected_index_subject.Value;
        if (cur_index < 0 || m_menu_item_datas.Length <= cur_index)
            return;



        var menu_type = m_menu_item_datas[cur_index].MenuType;

        switch (menu_type)
        {
            case MENU_ITEM_DATA.EnumMenuType.Attack:
                break;
            case MENU_ITEM_DATA.EnumMenuType.Wait:
                break;
            case MENU_ITEM_DATA.EnumMenuType.Skill:
                break;
            case MENU_ITEM_DATA.EnumMenuType.Item:
                break;                
        }
        
    }

}
