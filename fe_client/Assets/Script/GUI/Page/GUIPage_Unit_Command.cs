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
    typeof(GUI_Menu_CancelEvent),
    typeof(Battle_Scene_ChangeEvent),
    typeof(Battle_Command_Event)
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
            true,

            // is multiple open
            false                     
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
        

        public int                 Index    { get; private set; }
        public EnumUnitCommandType MenuType { get; private set; }
        // public string       Text  { get; private set; } = "";
        public MENU_ITEM_DATA(int _index, EnumUnitCommandType _type)
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
                case EnumUnitCommandType.Attack: 
                    table = "localization_base";
                    key   = "ui_menu_attack";                    
                    break;
                case EnumUnitCommandType.Wand:
                    table = "localization_base";
                    key   = "ui_menu_wand";
                    break;
                case EnumUnitCommandType.Skill:
                    table = "localization_base";
                    key   = "ui_menu_skill";
                    break;
                case EnumUnitCommandType.Item:
                    table = "localization_base";
                    key   = "ui_menu_item";
                    break;
                case EnumUnitCommandType.Exchange:
                    table = "localization_base";
                    key   = "ui_menu_item_exchange";
                    break;
                case EnumUnitCommandType.Wait:  
                    table = "localization_base";
                    key   = "ui_menu_wait";
                    break;
            }

            // ui_turn_number: Turn {0}

            return LocalizeKey.Create(table, key);
        }

        public static MENU_ITEM_DATA Empty => new MENU_ITEM_DATA(0, EnumUnitCommandType.None);
    }

    


    [SerializeField]
    private RectTransform                 m_grid_menu_root_bg_rect;

    [SerializeField]
    private RectTransform                 m_grid_menu_root_rect;

    [SerializeField]
    private GridLayoutGroup               m_grid_menu_root;

    [SerializeField]
    private GUIElement_Grid_Item_MenuText m_grid_menu_item;


    private Int64                         m_entity_id              = 0;           
    private List<MENU_ITEM_DATA>          m_menu_item_datas        = new();
    private (bool init, Vector2 value)    m_grid_menu_padding      = (false, Vector2.zero);  
    private BehaviorSubject<int>          m_selected_index_subject = new(0);

    private MENU_ITEM_DATA SelectedItemData
    {
        get
        {
            var cur_index = m_selected_index_subject.Value;
            if (cur_index < 0 || cur_index >= m_menu_item_datas.Count)
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

            case GUI_Menu_CancelEvent menu_cancel_event:
                OnReceiveEvent_GUI_Menu_CancelEvent(menu_cancel_event);
                break;

            case Battle_Command_Event battle_command_event:
                OnReceiveEvent_Battle_CommandEvent(battle_command_event);
                break;
        }
        // throw new NotImplementedException();
    }
    

    protected override void OnOpen(GUIOpenParam _param)
    {
        var param   = _param as PARAM;
        m_entity_id = param?.EntityID ?? 0;

        // 메뉴 아이템 그리기.
        UpdateMenuItems();  

        // 그리드 메뉴 레이아웃 업데이트.
        UpdateLayout();
    }

    protected override void OnLoop()
    {
        base.OnLoop();

        // 메뉴 타입에 따라 범위 그리기.
        UpdateDrawRange();
    }

    protected override void OnClose()
    {
        // 메뉴 타입에 따라 범위 초기화.
        BattleSystemManager.Instance.DrawRange.Clear();

        //  Debug.Log("GUIPage_Unit_Command: OnClose");
    }

    protected override void OnPostProcess_Close()
    {
        // Debug.Log("GUIPage_Unit_Command: OnPostProcess_Close");
        
    }



    private void UpdateMenuItems()
    {
        var entity = EntityManager.Instance.GetEntity(m_entity_id);
        if (entity == null)
            return;

        // 사용 가능한 무기목록 추출,
        // 사용 가능한 지팡이 목록 추출,
        // TODO: 스킬 목록 추출,
        // 소지한 아이템 목록 추출.

        using var list_weapon   = ListPool<Item>.AcquireWrapper();
        using var list_wand     = ListPool<Item>.AcquireWrapper();
        using var list_item     = ListPool<Item>.AcquireWrapper();
        // using var list_exchange = ListPool<Item>.AcquireWrapper();




        var enable_exchange = false;

        if (entity.HasCommandEnable(EnumCommandFlag.Action))
        {
            // 장착 가능한 무기 목록
            entity.Inventory.CollectItem_Weapon_Available(list_weapon.Value, entity);

            // 사용가능한 지팡이 목록
            entity.Inventory.CollectItem_Wand_Available(list_wand.Value, entity);

            // 교환 가능한지 체크.
            enable_exchange = entity.HasCommandEnable(EnumCommandFlag.Exchange);

            // 소지한 아이템 목록 추출
            entity.Inventory.CollectItem(list_item.Value);
        }

        // 메뉴 아이템 목록 초기화.
        m_menu_item_datas.Clear();

        int menu_index = 0;

        // 공격
        if (0 < list_weapon.Value.Count)    
            m_menu_item_datas.Add(new MENU_ITEM_DATA(menu_index++, EnumUnitCommandType.Attack));

        // 지팡이
        if (0 < list_wand.Value.Count)      
            m_menu_item_datas.Add(new MENU_ITEM_DATA(menu_index++, EnumUnitCommandType.Wand));

        // 교환
        if (enable_exchange)  
            m_menu_item_datas.Add(new MENU_ITEM_DATA(menu_index++, EnumUnitCommandType.Exchange));

        // 아이템
        if (0 < list_item.Value.Count)      
            m_menu_item_datas.Add(new MENU_ITEM_DATA(menu_index++, EnumUnitCommandType.Item));

        // 대기
        m_menu_item_datas.Add(new MENU_ITEM_DATA(menu_index++, EnumUnitCommandType.Wait));

        // 메뉴 아이템 그리기.
        m_grid_menu_root.transform.DestroyAllChildren();
        for (int i = 0; i < m_menu_item_datas.Count; i++)
        {
            var localizeKey  = m_menu_item_datas[i].GetLocalizeKey();
            var text_subject = LocalizationManager.Instance.GetTextObservable(localizeKey.Table, localizeKey.Key);
            var clonedItem   = Instantiate(m_grid_menu_item, m_grid_menu_root.transform);
            
            clonedItem.Initialize(i, m_selected_index_subject, text_subject);
        }

        ClampSelectIndex();
    }

    private void ClampSelectIndex()
    {
        var cur_index = m_selected_index_subject.Value;
        var new_index = Math.Clamp(cur_index, 0, Math.Max(0, m_menu_item_datas.Count - 1));

        if (new_index != cur_index)
            m_selected_index_subject.OnNext(new_index);
    }

    private void UpdateLayout()
    {
        // 그리드 메뉴 패딩 계산.
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

    // // 선택 인덱스 설정
    // public void SetSelectedIndex(int index)
    // {
    //     m_selected_index_subject.OnNext(index);
    // }

    
    // 메뉴 이동 이벤트.
    void OnReceiveEvent_GUI_Menu_MoveEvent(GUI_Menu_MoveEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        // 메뉴 이동 방향이 없으면 종료.
        if (_event.MoveDirection.y  == 0)
            return;

        
        // 메뉴 이동 방향에 따라 인덱스 추가.
        var add_index = _event.MoveDirection.y > 0 ? -1 : +1;
        var cur_index = m_selected_index_subject.Value;
        var new_index = cur_index + add_index;

        // 인덱스 클램프.
        new_index     = Math.Clamp(new_index, 0, m_menu_item_datas.Count - 1);
        
        // 선택 인덱스 설정.
        m_selected_index_subject.OnNext(new_index);

        ClampSelectIndex();

        // 공격 범위 탐색.
        UpdateDrawRange();
    }

    void OnReceiveEvent_GUI_Menu_SelectEvent(GUI_Menu_SelectEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        // 선택 메뉴 타입에 따라 처리.        
        switch (SelectedItemData.MenuType)
        {
            case EnumUnitCommandType.Attack:
            {
                // 공격 GUI 오픈.
                GUIManager.Instance.OpenUI(
                    GUIPage_Unit_Command_Attack.PARAM.Create(m_entity_id, EnumUnitCommandType.Attack)
                    );
            }
                break;
            case EnumUnitCommandType.Wand:
            {
                // 지팡이 GUI 오픈.
                GUIManager.Instance.OpenUI(
                    GUIPage_Unit_Command_Attack.PARAM.Create(m_entity_id, EnumUnitCommandType.Wand)
                    );
            }
                break;
            case EnumUnitCommandType.Wait:   
            {
                // 대기 명령 추가.
                BattleSystemManager.Instance.PushCommand(
                    new Command_Done(m_entity_id)
                    );

                // GUI 닫기.
                GUIManager.Instance.CloseUI(ID);
            }             
                break;
            case EnumUnitCommandType.Skill:
                break;
            case EnumUnitCommandType.Item:
                break;  
            case EnumUnitCommandType.Exchange:
            {
                // 교환 GUI 오픈.
                GUIManager.Instance.OpenUI(
                    GUIPage_Unit_Command_Exchange.PARAM.Create(m_entity_id)
                    );
            }
                break;              
        }

        
    }

    void OnReceiveEvent_GUI_Menu_CancelEvent(GUI_Menu_CancelEvent _event)
    {
        if (_event == null || _event.GUI_ID != ID)
            return;

        var entity = EntityManager.Instance.GetEntity(m_entity_id);
        if (entity != null && entity.HasCommandEnable(EnumCommandFlag.Move) == false)
        {
            // 이동이 불가능할 경우 취소로 UI를 닫을수 없음.            
            return;
        }

        GUIManager.Instance.CloseUI(ID);
    }

    protected override void OnVisibleChanged(bool _visible)
    {
        base.OnVisibleChanged(_visible);

        if (_visible)
        {
            // 메뉴 아이템 그리기.
            UpdateMenuItems();  

            // 그리드 메뉴 레이아웃 업데이트.
            UpdateLayout();
        }
    }

    private void UpdateDrawRange()
    {
        if (IsInputFocused == false)
            return;

        int draw_flag = 0;
        switch (SelectedItemData.MenuType)
        {
            case EnumUnitCommandType.Attack:
                draw_flag = (int)Battle.MoveRange.EnumDrawFlag.AttackRange;
                break;
            case EnumUnitCommandType.Wand:
                draw_flag = (int)Battle.MoveRange.EnumDrawFlag.WandRange;
                break;
            case EnumUnitCommandType.Exchange:
                draw_flag = (int)Battle.MoveRange.EnumDrawFlag.ExchangeRange;
                break;
        }

        // 메뉴 타입에 따라 범위 그리기.
        BattleSystemManager.Instance.DrawRange.DrawRange(
            _draw_flag: draw_flag,
            _entityID: m_entity_id,
            _use_base_position: false);

    }

    void OnReceiveEvent_Battle_CommandEvent(Battle_Command_Event _event)
    {
        if (_event == null || _event.EntityID != m_entity_id)
            return;

        UpdateMenuItems();
        UpdateLayout();
    }

}
